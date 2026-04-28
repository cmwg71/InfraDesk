using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InfraDesk.Infrastructure.Persistence;

namespace InfraDesk.Infrastructure;

public static class DependencyInjection
{
    // Diese Methode "erweitert" das Services-Objekt der API
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Wir registrieren den DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Später registrieren wir hier auch Repositories oder E-Mail-Services
        return services;
    }
}