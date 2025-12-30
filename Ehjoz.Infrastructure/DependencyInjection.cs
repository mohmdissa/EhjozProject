using Microsoft.Extensions.DependencyInjection;
using EhjozProject.Application.Interfaces;
using EhjozProject.Application.Services;
using EhjozProject.Domain.Interfaces.Repositories;
using EhjozProject.Infrastructure.Repositories;

namespace EhjozProject.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IStadiumRepository, StadiumRepository>();

            // Register Services
            services.AddScoped<IStadiumService, StadiumService>();

            return services;
        }
    }
}