using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supabase;
using vet_api_Net.Data;
using vet_api_Net.Hubs;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Middleware;
using vet_api_Net.Services;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IInvoiceService, InvoiceService>();

builder.Services.AddInfrastructureServices(builder.Configuration);

var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];
if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
{
    throw new Exception($"[ERROR] Las credenciales de Supabase no se cargaron correctamente de appsettings.json. URL: '{supabaseUrl}', Key cargada: {(string.IsNullOrEmpty(supabaseKey) ? "NO" : "SI")}");
}
builder.Services.AddScoped<global::Supabase.Client>(_ =>
    new global::Supabase.Client(
        supabaseUrl,
        supabaseKey,
        new global::Supabase.SupabaseOptions
        {
            AutoRefreshToken = true
        }));

builder.Services.AddScoped<vet_api_Net.Interfaze.Services.ISupabaseService, vet_api_Net.Services.Supabase.SupabaseService>();

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

app.Use(async (context, next) =>
{
    app.Logger.LogDebug("{Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
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