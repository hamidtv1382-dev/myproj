using AutoMapper;
using Order_Service.src._01_Domain.Core.Entities;
using Order_Service.src._01_Domain.Core.Interfaces.UnitOfWork;
using Order_Service.src._01_Domain.Core.ValueObjects;
using Order_Service.src._01_Domain.Services.Interfaces;
using Order_Service.src._02_Application.DTOs.Requests;
using Order_Service.src._02_Application.DTOs.Responses;
using Order_Service.src._02_Application.Exceptions;
using Order_Service.src._02_Application.Services.Interfaces;

namespace Order_Service.src._02_Application.Services.Implementations
{
    public class BasketApplicationService : IBasketApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountService _discountService;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketApplicationService> _logger;

        public BasketApplicationService(
            IUnitOfWork unitOfWork,
            IDiscountService discountService,
            IMapper mapper,
            ILogger<BasketApplicationService> logger)
        {
            _unitOfWork = unitOfWork;
            _discountService = discountService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BasketDetailResponseDto> GetBasketAsync(Guid buyerId)
        {
            var basket = await _unitOfWork.Baskets.GetByBuyerIdAsync(buyerId);

            if (basket == null)
            {
                // Return empty basket representation
                return new BasketDetailResponseDto { Id = Guid.Empty, Items = new List<BasketDetailResponseDto.BasketItemResponseDto>(), TotalAmount = 0 };
            }

            return _mapper.Map<BasketDetailResponseDto>(basket);
        }

        public async Task<BasketDetailResponseDto> AddItemAsync(Guid buyerId, AddItemToBasketRequestDto request)
        {
            var basket = await _unitOfWork.Baskets.GetByBuyerIdAsync(buyerId);

            if (basket == null)
            {
                // Create new basket if not exists
                basket = new Basket(Guid.NewGuid(), buyerId, TimeSpan.FromDays(7));
                await _unitOfWork.Baskets.AddAsync(basket);
            }

            // NOTE: In a real app, you must fetch Product Details (Name, Price) from Catalog Service here.
            // Mocking product details for this snippet:
            var productName = "Sample Product";
            var unitPrice = new Money(100000); // 100,000 IRR
            var imageUrl = "https://example.com/image.jpg";

            var item = new BasketItem(Guid.NewGuid(), basket.Id, request.ProductId, productName, imageUrl, unitPrice, request.Quantity);
            basket.AddItem(item);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<BasketDetailResponseDto>(basket);
        }

        public async Task<BasketDetailResponseDto> UpdateItemAsync(Guid buyerId, UpdateBasketItemRequestDto request)
        {
            var basket = await _unitOfWork.Baskets.GetByBuyerIdAsync(buyerId);
            if (basket == null)
                throw new BasketNotFoundException(buyerId.ToString()); // Conceptual exception

            basket.UpdateItemQuantity(request.ProductId, request.Quantity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BasketDetailResponseDto>(basket);
        }

        public async Task<BasketDetailResponseDto> RemoveItemAsync(Guid buyerId, RemoveItemFromBasketRequestDto request)
        {
            var basket = await _unitOfWork.Baskets.GetByBuyerIdAsync(buyerId);
            if (basket == null)
                throw new BasketNotFoundException(buyerId.ToString());

            basket.RemoveItem(request.ProductId);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BasketDetailResponseDto>(basket);
        }

        public async Task<BasketDetailResponseDto> ApplyDiscountAsync(Guid buyerId, ApplyDiscountRequestDto request)
        {
            var basket = await _unitOfWork.Baskets.GetByBuyerIdAsync(buyerId);
            if (basket == null)
                throw new BasketNotFoundException(buyerId.ToString());

            var discount = await _discountService.ValidateDiscountAsync(request.DiscountCode, basket.TotalAmount);
            if (discount == null)
                throw new InvalidDiscountCodeException(request.DiscountCode);

            basket.ApplyDiscount(discount.Id);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BasketDetailResponseDto>(basket);
        }

        public async Task ClearBasketAsync(Guid buyerId)
        {
            var basket = await _unitOfWork.Baskets.GetByBuyerIdAsync(buyerId);
            if (basket != null)
            {
                basket.Clear();
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
