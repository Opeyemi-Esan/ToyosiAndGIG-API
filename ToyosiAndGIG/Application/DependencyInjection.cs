using ToyosiAndGIG.Application.Interface;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Application.Services;
using ToyosiAndGIG.Infrastructure.Repository;

namespace ToyosiAndGIG.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<Helper>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IBookingService, BookingService>();
            return services;
        }
    }
}
