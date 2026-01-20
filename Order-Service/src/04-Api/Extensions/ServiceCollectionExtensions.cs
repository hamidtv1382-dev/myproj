using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order_Service.src._01_Domain.Core.Interfaces.Repositories;
using Order_Service.src._01_Domain.Core.Interfaces.UnitOfWork;
using Order_Service.src._01_Domain.Services.Implementations;
using Order_Service.src._01_Domain.Services.Interfaces;
using Order_Service.src._02_Application.Interfaces;
using Order_Service.src._02_Application.Services.Implementations;
using Order_Service.src._02_Application.Services.Interfaces;
using Order_Service.src._03_Infrastructure.Caching;
using Order_Service.src._03_Infrastructure.Data;
using Order_Service.src._03_Infrastructure.Repositories;
using Order_Service.src._03_Infrastructure.Services.External;

namespace Order_Service.src._04_Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Application Services
            services.AddScoped<IOrderApplicationService, OrderApplicationService>();
            services.AddScoped<IBasketApplicationService, BasketApplicationService>();
            services.AddScoped<IPaymentApplicationService, PaymentApplicationService>();
            services.AddScoped<IRefundApplicationService, RefundApplicationService>();

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderDomainService, OrderDomainService>();
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<IJwtTokenValidationService, JwtTokenValidationService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IRefundRepository, RefundRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // External Services (HTTP Clients)
            services.AddHttpClient<CatalogServiceClient>();
            services.AddHttpClient<PaymentServiceClient>();
            services.AddHttpClient<SellerFinanceServiceClient>();

            // Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "OrderService_";
            });
            services.AddSingleton<ICacheService, RedisCacheService>();

            return services;
        }

        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });

            return services;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
