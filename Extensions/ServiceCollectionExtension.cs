namespace AuthService.Extensions;

using UserContext;
using Microsoft.EntityFrameworkCore;
using Entities;
using Models.Validation;
using Models;
using Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

public static class ServiceCollectionExtension
{
    public static void AddAuthService(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        JwtAppSettings jwtConfig = new JwtAppSettings();
        var jwtAppSettings = configuration.GetSection("Auth");
        jwtAppSettings.Bind(jwtConfig);

        services.Configure<JwtAppSettings>(jwtAppSettings);

        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("App"));
        });

        // Add services to the container.
        services.AddScoped<IJwtManager, JwtManager>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDeviceService, DeviceService>();

        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddScoped<IValidator<UserBodyResponse>, UserBodyResponseValidation>();
        services.AddScoped<IValidator<UserLoginBody>, UserLoginBodyValidation>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
        services.AddScoped<IUserContext, UserContext>();

        services.AddCors(options =>
        {
            options.AddPolicy(name: "apiCorsPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        //                .AllowCredentials()
                        .SetIsOriginAllowed(options => true);
                    //.WithMethods("OPTIONS", "GET");
                });
        });
        services.AddHttpContextAccessor();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg =>
        {
            RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(
                source: Convert.FromBase64String(jwtConfig.JwtPublicKey),
                bytesRead: out int _
            );
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,

                IssuerSigningKey = new RsaSecurityKey(rsa),
            };
        });
        services.AddScoped<Seeder>();
    }
}