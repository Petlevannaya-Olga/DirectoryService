using DirectoryService.Application;
using DirectoryService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Primitives;
using Serilog;
using Serilog.Exceptions;

namespace DirectoryService.Presentation.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DirectoryServiceDb")));

        services
            .AddSerilogLogging(configuration)
            .AddWebDependencies()
            .AddApplication()
            .AddInfrastructure();

        return services;
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer((schema, context, _) =>
            {
                if (context.JsonTypeInfo.Type == typeof(Envelope<Errors>))
                {
                    if (schema.Properties.TryGetValue("errors", out OpenApiSchema? errorsProperty))
                    {
                        errorsProperty.Items.Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema, Id = "Error",
                        };
                    }
                }

                return Task.CompletedTask;
            });
        });
        return services;
    }

    private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((s, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(s)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("ServiceName", "DirectoryService"));

        return services;
    }
}