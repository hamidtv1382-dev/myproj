using Seller_Finance_Service.src._01_Domain.Core.ValueObjects;
using Seller_Finance_Service.src._02_Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace Seller_Finance_Service.src._03_Infrastructure.Services.External
{
    public class FinanceServiceClient : IFinanceService
    {
        private readonly HttpClient _httpClient;

        public FinanceServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> TransferToSellerAccountAsync(BankAccountInfo bankInfo, Money amount)
        {
            // Call Finance Service to perform bank transfer
            var payload = new
            {
                BankAccount = bankInfo,
                Amount = amount.Amount
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://finance-service/api/settlements/process", content);
            return response.IsSuccessStatusCode;
        }
    }
}
