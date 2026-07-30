using DirectoryService.Application;
using DirectoryService.Infrastructure;
using Microsoft.OpenApi;
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
        services
            .AddSerilogLogging(configuration)
            .AddWebDependencies()
            .AddApplication()
            .AddInfrastructure(configuration);

        return services;
    }

    private static IServiceCollection AddWebDependencies(
        this IServiceCollection services)
    {
        services.AddControllers();
        services.AddHealthChecks();

        services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer(
                async (schema, context, cancellationToken) =>
                {
                    if (context.JsonTypeInfo.Type != typeof(Envelope<Errors>))
                    {
                        return;
                    }

                    if (schema.Properties is null ||
                        !schema.Properties.TryGetValue(
                            "errors",
                            out IOpenApiSchema? errorsProperty) ||
                        errorsProperty is not OpenApiSchema errorsSchema)
                    {
                        return;
                    }

                    OpenApiSchema errorSchema =
                        await context.GetOrCreateSchemaAsync(
                            typeof(Error),
                            parameterDescription: null,
                            cancellationToken);

                    context.Document?.AddComponent(
                        "Error",
                        errorSchema);

                    errorsSchema.Items =
                        new OpenApiSchemaReference(
                            "Error",
                            context.Document);
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