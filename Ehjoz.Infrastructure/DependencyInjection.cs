using EhjozProject.Application.Interfaces;
using EhjozProject.Application.Services;
using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EhjozProject.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IStadiumRepository, StadiumRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            // Register Services
            services.AddScoped<IStadiumService, StadiumService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<ITimeSlotService, TimeSlotService>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}