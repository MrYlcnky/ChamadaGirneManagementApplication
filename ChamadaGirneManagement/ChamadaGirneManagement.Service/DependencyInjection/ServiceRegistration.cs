using ChamadaGirneManagement.Service.Interfaces.Services;
using ChamadaGirneManagement.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChamadaGirneManagement.Service.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddServiceServices(
        this IServiceCollection services
    )
    {
        services.AddScoped<
            IYonetimKullanicisiService,
            YonetimKullanicisiService
        >();

        services.AddScoped<
            IUygulamaService,
            UygulamaService
        >();

        services.AddScoped<
            IUygulamaGecisService,
            UygulamaGecisService
        >();

        services.AddScoped<
            IJwtTokenService,
            JwtTokenService
        >();

        services.AddScoped<
            IKimlikDogrulamaService,
            KimlikDogrulamaService
        >();

        return services;
    }
}