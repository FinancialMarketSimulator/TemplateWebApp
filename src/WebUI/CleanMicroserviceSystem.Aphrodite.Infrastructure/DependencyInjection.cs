﻿using System.IdentityModel.Tokens.Jwt;
using CleanMicroserviceSystem.Aphrodite.Application.Configurations;
using CleanMicroserviceSystem.Aphrodite.Domain;
using CleanMicroserviceSystem.Aphrodite.Infrastructure.Services;
using CleanMicroserviceSystem.Aphrodite.Infrastructure.Services.Authentication;
using CleanMicroserviceSystem.Authentication.Application;
using CleanMicroserviceSystem.Authentication.Domain;
using CleanMicroserviceSystem.Themis.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CleanMicroserviceSystem.Aphrodite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureServices(
        this IServiceCollection services,
        AphroditeConfiguration configuration)
    {
        _ = services.AddMasaBlazor();
        _ = services.AddLogging();
        _ = services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(IdentityContract.ThemisAPIReadPolicyName, IdentityContract.ThemisAPIReadPolicy);
            options.AddPolicy(IdentityContract.ThemisAPIWritePolicyName, IdentityContract.ThemisAPIWritePolicy);
        });
        _ = services
            .AddSingleton<CookieStorage>()
            .AddSingleton<JwtSecurityTokenHandler>()
            .AddSingleton<IAuthenticationTokenStore, AphroditeAuthenticationTokenStore>()
            .AddSingleton<AuthenticationStateProvider, AphroditeAuthenticationStateProvider>()
            .AddSingleton<AphroditeJwtSecurityTokenValidator>();
        _ = services.AddHttpClient(
            ApiContract.AphroditeHttpClientName,
            client => client.BaseAddress = new Uri(configuration.WebUIBaseAddress));
        _ = services
            .AddThemisClients(new ThemisClientConfiguration()
            {
                GatewayClientName = ApiContract.GatewayHttpClientName,
            })
            .AddTransient<DefaultAuthenticationDelegatingHandler>()
            .AddHttpClient<HttpClient>(
                ApiContract.GatewayHttpClientName,
                client => client.BaseAddress = new Uri(configuration.GatewayBaseAddress))
            .AddHttpMessageHandler<DefaultAuthenticationDelegatingHandler>();
        return services;
    }
}
