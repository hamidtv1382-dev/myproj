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
    public class OrderApplicationService : IOrderApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderDomainService _orderDomainService;
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderApplicationService> _logger;

        public OrderApplicationService(
            IUnitOfWork unitOfWork,
            IOrderDomainService orderDomainService,
            IInventoryService inventoryService,
            IMapper mapper,
            ILogger<OrderApplicationService> logger)
        {
            _unitOfWork = unitOfWork;
            _orderDomainService = orderDomainService;
            _inventoryService = inventoryService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDetailResponseDto> CreateOrderAsync(Guid buyerId, CreateOrderRequestDto request)
        {
            var basket = await _unitOfWork.Baskets.GetByBuyerIdAsync(buyerId);
            if (basket == null || !basket.Items.Any())
                throw new ArgumentException("Basket is empty.");

            var shippingAddress = new ShippingAddress(
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Country,
                request.City,
                request.State,
                request.Street,
                request.ZipCode,
                request.BuildingNumber,
                request.ApartmentNumber
            );

            var orderItems = new List<OrderItem>();
            foreach (var basketItem in basket.Items)
            {
                orderItems.Add(new OrderItem(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    basketItem.ProductId,
                    basketItem.ProductName,
                    basketItem.ImageUrl,
                    basketItem.UnitPrice,
                    basketItem.Quantity
                ));
            }

            var order = await _orderDomainService.CreateOrderAsync(buyerId, shippingAddress, orderItems, request.DiscountCode);

            foreach (var item in order.Items)
            {
                var reserved = await _inventoryService.ReserveStockAsync(item.ProductId, item.Quantity);
                if (!reserved)
                {
                    foreach (var reservedItem in order.Items.Where(i => i.ProductId != item.ProductId))
                    {
                        await _inventoryService.ReleaseStockAsync(reservedItem.ProductId, reservedItem.Quantity);
                    }
                    throw new InsufficientStockException(item.ProductId, item.Quantity);
                }
            }

            await _unitOfWork.Orders.AddAsync(order);
            basket.Clear();
            _unitOfWork.Baskets.Update(basket);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDetailResponseDto>(order);
        }

        public async Task<OrderDetailResponseDto> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId);

            return _mapper.Map<OrderDetailResponseDto>(order);
        }

        public async Task<IEnumerable<OrderSummaryResponseDto>> GetOrdersByBuyerIdAsync(Guid buyerId)
        {
            var orders = await _unitOfWork.Orders.GetByBuyerIdAsync(buyerId);
            return _mapper.Map<IEnumerable<OrderSummaryResponseDto>>(orders);
        }

        public async Task<OrderDetailResponseDto> UpdateOrderAsync(UpdateOrderRequestDto request)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
            if (order == null)
                throw new OrderNotFoundException(request.OrderId);

            var newAddress = new ShippingAddress(
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Country,
                request.City,
                request.State,
                request.Street,
                request.ZipCode,
                request.BuildingNumber,
                request.ApartmentNumber
            );

            order.UpdateShippingAddress(newAddress);

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDetailResponseDto>(order);
        }

        public async Task<OrderDetailResponseDto> CancelOrderAsync(CancelOrderRequestDto request)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
            if (order == null)
                throw new OrderNotFoundException(request.OrderId);

            _orderDomainService.CancelOrder(order, request.Reason ?? "User requested cancellation");

            foreach (var item in order.Items)
            {
                await _inventoryService.ReleaseStockAsync(item.ProductId, item.Quantity);
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDetailResponseDto>(order);
        }

        public async Task<TrackOrderResponseDto> TrackOrderAsync(TrackOrderRequestDto request)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
            if (order == null)
                throw new OrderNotFoundException(request.OrderId);

            // Mock timeline logic based on status and dates
            var response = new TrackOrderResponseDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber.Value,
                CurrentStatus = order.Status.ToString(),
                EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5), // Fixed type issue
                History = new List<TrackOrderResponseDto.OrderTimelineDto>
                {
                    new TrackOrderResponseDto.OrderTimelineDto { Status = "Pending", Date = order.CreatedAt },
                    new TrackOrderResponseDto.OrderTimelineDto { Status = order.Status.ToString(), Date = order.UpdatedAt ?? order.CreatedAt }
                }
            };

            return response;
        }
    }
}