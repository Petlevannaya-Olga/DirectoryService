using Serilog;

namespace DirectoryService.Presentation.Configuration;

public static class AppExtensions
{
    public static IApplicationBuilder UseWebDependencies(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Directory service API"));
        }

        app.MapControllers();

        return app;
    }
}