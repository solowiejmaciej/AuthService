using AuthService;
using AuthService.Middleware;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using AuthService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthService();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddLogging();
builder.Services.AddHealthChecks();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService", Version = "v1" });

    // Konfiguracja autoryzacji JWT
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

var app = builder.Build();
app.UseCors("apiCorsPolicy"); // allow credentials

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

SeedDatabase();

app.MapHealthChecks("/health");

app.Run();

void SeedDatabase() //can be placed at the very bottom under app.Run()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<Seeder>();
        dbInitializer.Seed();
    }
}