﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanMicroserviceSystem.Authentication.Configurations;
using CleanMicroserviceSystem.Authentication.Domain;
using Microsoft.IdentityModel.Tokens;

namespace CleanMicroserviceSystem.Authentication.Services;

public class JwtBearerTokenGenerator : IJwtBearerTokenGenerator
{
    private readonly IOptionsSnapshot<JwtBearerConfiguration> options;
    private readonly SymmetricSecurityKey securityKey;
    private readonly SigningCredentials signingCredentials;
    private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

    public JwtBearerTokenGenerator(
        IOptionsSnapshot<JwtBearerConfiguration> options)
    {
        this.options = options;
        this.securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.options.Value.JwtSecurityKey));
        this.signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateUserSecurityToken(IEnumerable<Claim> claims)
    {
        var expiry = DateTime.UtcNow.AddMinutes(this.options.Value.JwtExpiryForUser);
        return this.GenerateSecurityToken(claims, expiry);
    }

    public string GenerateClientSecurityToken(IEnumerable<Claim> claims)
    {
        var expiry = DateTime.UtcNow.AddMinutes(this.options.Value.JwtExpiryForClient);
        return this.GenerateSecurityToken(claims, expiry);
    }

    protected string GenerateSecurityToken(IEnumerable<Claim> claims, DateTime expiry)
    {
        var token = this.jwtSecurityTokenHandler.CreateJwtSecurityToken(
            this.options.Value.JwtIssuer,
            this.options.Value.JwtAudience,
            new ClaimsIdentity(claims, IdentityContract.JwtAuthenticationType),
            DateTime.UtcNow,
            expiry,
            DateTime.UtcNow,
            signingCredentials: this.signingCredentials);
        return this.jwtSecurityTokenHandler.WriteToken(token);
    }
}
