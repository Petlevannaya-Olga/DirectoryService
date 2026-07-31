using System.Globalization;
using DirectoryService.Presentation.Configuration;
using DotNetEnv;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("Приложение запускается");

    Env
        .NoClobber()
        .TraversePath()
        .Load();
    
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddProgramDependencies(builder.Configuration);

    var app = builder.Build();
    app.UseWebDependencies();
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Не удалось запустить приложение ");
}
finally
{
    Log.CloseAndFlush();
}