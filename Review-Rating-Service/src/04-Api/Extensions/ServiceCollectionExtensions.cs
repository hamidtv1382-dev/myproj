using Microsoft.EntityFrameworkCore;
using Review_Rating_Service.src._01_Domain.Core.Interfaces.Repositories;
using Review_Rating_Service.src._01_Domain.Core.Interfaces.UnitOfWork;
using Review_Rating_Service.src._01_Domain.Services.Implementations;
using Review_Rating_Service.src._01_Domain.Services.Interfaces;
using Review_Rating_Service.src._02_Application.Interfaces;
using Review_Rating_Service.src._02_Application.Services.Implementations;
using Review_Rating_Service.src._02_Application.Services.Interfaces;
using Review_Rating_Service.src._03_Infrastructure.Data;
using Review_Rating_Service.src._03_Infrastructure.Repositories;
using Review_Rating_Service.src._03_Infrastructure.Services.External;
using static System.Net.Mime.MediaTypeNames;

namespace Review_Rating_Service.src._04_Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReviewServices(this IServiceCollection services, string connectionString)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repositories & Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();

            // Application Services
            services.AddScoped<IReviewApplicationService, ReviewApplicationService>();
            services.AddScoped<IRatingApplicationService, RatingApplicationService>();

            // Domain Services
            services.AddScoped<IReviewDomainService, ReviewDomainService>();
            services.AddScoped<IRatingDomainService, RatingDomainService>();

            // External Services
            services.AddHttpClient<IExternalNotificationService, NotificationClient>();

            // AutoMapper
            services.AddAutoMapper(config =>
            {
                config.AddMaps(System.Reflection.Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}
