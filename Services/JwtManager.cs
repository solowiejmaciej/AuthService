using AuthService.Entities;
using AuthService.Exceptions;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using AuthService.Models;

namespace AuthService.Services;

public interface IJwtManager
{
    TokenResponse GenerateJWT(UserBodyResponse user);
}

public class JwtManager : IJwtManager
{
    private UserDbContext _dbcontext { get; }
    private IPasswordHasher<User> _passwordHasher { get; }
    private JwtAppSettings _jwtAppSettings { get; }

    public JwtManager(UserDbContext dbcontext, IPasswordHasher<User> passwordHasher, JwtAppSettings jwtSettings)
    {
        _jwtAppSettings = jwtSettings;
        _dbcontext = dbcontext;
        _passwordHasher = passwordHasher;
    }

    public TokenResponse GenerateJWT(UserBodyResponse dto)
    {
        var user = _dbcontext.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(x => x.Login == dto.Login);
        if (user is null)
        {
            throw new BadRequestException("Invalid username or password");
        }
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHashed, dto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var expires = DateTime.UtcNow.AddDays(_jwtAppSettings.JwtExpireDays);

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}")
            };

        string rsaPrivateKey = File.ReadAllText(@"certi\privateKey.pem");
        using var rsa = RSA.Create();
        rsa.ImportFromPem(rsaPrivateKey);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var jwt = new JwtSecurityToken(
            audience: _jwtAppSettings.JwtIssuer,
            issuer: _jwtAppSettings.JwtIssuer,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        var response = new TokenResponse()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            StatusCode = 200,
            IssuedDate = DateTime.UtcNow,
            ExpiresAt = expires,
            Role = user.Role.Name
        };

        return response;
    }
}