using DirectoryService.Application;
using DirectoryService.Application.Database;
using DirectoryService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Строка подключения 'Database' не найдена.");

        services.AddDbContext<ApplicationDbContext>(
            options => options.UseNpgsql(connectionString));

        services.AddScoped<IReadDbContext>(
            serviceProvider =>
                serviceProvider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ILocationsRepository, LocationsRepository>();
        services.AddScoped<IPositionsRepository, PositionsRepository>();
        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();

        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
}