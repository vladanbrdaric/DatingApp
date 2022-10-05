using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    // class have to be static to be able to create extension methods.
    public static class ApplicationServiceExtensions
    {
        // method have to be static
        // I have to specify return type
        // I have to use 'this' word and then 'type' that I'm extending.
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // scoped is most appropriate for a HTTP request.
            services.AddScoped<ITokenService, TokenService>();

            // Add DbContext service
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            // return services
            return services;
        }
    }
}