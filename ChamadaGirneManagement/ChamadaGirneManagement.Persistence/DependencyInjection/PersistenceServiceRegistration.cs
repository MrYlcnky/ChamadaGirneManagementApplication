using ChamadaGirneManagement.Persistence.Context;
using ChamadaGirneManagement.Persistence.Repositories;
using ChamadaGirneManagement.Service.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChamadaGirneManagement.Persistence.DependencyInjection;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection bağlantı bilgisi bulunamadı."
            );

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            );
        });

        services.AddScoped<
            IYonetimKullanicisiRepository,
            YonetimKullanicisiRepository
        >();

        services.AddScoped<
            IUygulamaRepository,
            UygulamaRepository
        >();

        services.AddScoped<
            IUygulamaGecisKoduRepository,
            UygulamaGecisKoduRepository
        >();

        return services;
    }
}