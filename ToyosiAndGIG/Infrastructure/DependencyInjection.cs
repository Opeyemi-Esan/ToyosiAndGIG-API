using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Application.Services;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Infrastructure.Auth;
using ToyosiAndGIG.Infrastructure.Data;
using ToyosiAndGIG.Infrastructure.Email;
using ToyosiAndGIG.Infrastructure.Identity;
using ToyosiAndGIG.Infrastructure.Interface;
using ToyosiAndGIG.Infrastructure.Repository;
using ToyosiAndGIG.Infrastructure.Storage;

namespace ToyosiAndGIG.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;

                // Lockout policy actually enforces the lockoutOnFailure: true you already pass
                // into CheckPasswordSignInAsync. Without this config, that flag does nothing.
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
                return new Cloudinary(account);
            });

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenServices>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenServices>();
            services.AddScoped<IAccountConfirmationRepository, AccountConfirmationRepository>();
            services.AddScoped<IEmailServices, GmailSmtpEmailServices>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();
            services.AddScoped<IImageCompressionService, ImageCompressionService>();
            services.AddScoped<IBookingRepository, BookingRepository>();


            return services;
        }

    }
}
