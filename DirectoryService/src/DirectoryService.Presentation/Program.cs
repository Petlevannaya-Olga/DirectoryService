using DirectoryService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
//builder.Services.AddScoped<ApplicationDbContext>(_ =>
//    new ApplicationDbContext(builder.Configuration.GetConnectionString("DirectoryService")!));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DirectoryServiceDb")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/vi.json", "Directory service API"));
}

app.Run();