using AuthService.Entities;
using AuthService.Exceptions;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthService.Services;

public interface IJwtManager
{
    TokenResponse GenerateJWT(UserLoginBody user);
}

public class JwtManager : IJwtManager
{
    private UserDbContext _dbcontext { get; }
    private IPasswordHasher<ApplicationUser> _passwordHasher { get; }
    private JwtAppSettings _jwtAppSettings { get; }

    public JwtManager(UserDbContext dbcontext, IPasswordHasher<ApplicationUser> passwordHasher, JwtAppSettings jwtSettings)
    {
        _jwtAppSettings = jwtSettings;
        _dbcontext = dbcontext;
        _passwordHasher = passwordHasher;
    }

    public TokenResponse GenerateJWT(UserLoginBody dto)
    {
        var user = _dbcontext.Users.FirstOrDefault(x => x.Email == dto.Email);
        if (user is null)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var expires = DateTime.UtcNow.AddDays(_jwtAppSettings.JwtExpireDays);

        var userRoleId = _dbcontext.UserRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
        var userRoleName = _dbcontext.Roles.FirstOrDefault(r => r.Id == userRoleId).Name;

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, $"{userRoleName}")
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
            Role = userRoleName,
            RoleId = userRoleId
        };

        return response;
    }
}