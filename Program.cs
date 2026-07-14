using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using vet_api_Net.Data;
using vet_api_Net.Hubs;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Services;
using Microsoft.Extensions.Options;
using vet_api_Net.Infrastructure.Configuration;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IInvoiceService, InvoiceService>();

builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

if (Environment.GetEnvironmentVariable("BOOTSTRAP_DB") == "1")
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    Console.WriteLine("Database ensured (EnsureCreated).");
    return;
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var apiSettings = scope.ServiceProvider.GetRequiredService<IOptions<ApiSettingsOptions>>().Value;
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var seedDataOptions = configuration.GetSection("SeedData").Get<SeedDataOptions>() ?? new SeedDataOptions();
    if (seedDataOptions.Initialize)
    {
        vet_api_Net.Data.seeds.SeedsData.Initialize(db, apiSettings, seedDataOptions);
    }
}

app.Use(async (context, next) => {
    Console.WriteLine($"[req] {context.Request.Method} {context.Request.Path}");
    if (context.Request.Method != "OPTIONS")
    {
        await Task.Delay(100);
    }
    await next();
});

app.UseExceptionHandler(errorApp => {
    errorApp.Run(async context => {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Error en la solicitud procesada." });
    });
});

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseCors("HappyPetsPolicy");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MessageHub>("/hubs/mensajes");
app.MapHub<NotificactionsPush>("/hubs/notificaciones");
app.MapHealthChecks("/health");

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Bienvenido a Happy Pets API {EnvironmentName}. Orígenes CORS: {LocalOrigin}, {TunnelOrigin}", 
    app.Environment.EnvironmentName, 
    builder.Configuration["ConnectionSettings:LocalOrigin"], 
    builder.Configuration["ConnectionSettings:TunnelOrigin"]);

app.Run();