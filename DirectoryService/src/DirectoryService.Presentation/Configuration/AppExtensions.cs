using DirectoryService.Presentation.Extensions;
using Scalar.AspNetCore;
using Serilog;

namespace DirectoryService.Presentation.Configuration;

public static class AppExtensions
{
    public static IApplicationBuilder UseWebDependencies(this WebApplication app)
    {
        app.UseExceptionMiddleware();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Directory service API"));
        }

        app.MapControllers();
        app.MapHealthChecks("/health");

        return app;
    }
}