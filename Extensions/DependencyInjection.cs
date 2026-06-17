using System;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using vet_api_Net.Data;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.HttpServices;
using vet_api_Net.Worker;
using vet_api_Net.Services; 
using vet_api_Net.Repositories;
using vet_api_Net.Workers;
using vet_api_Net.Services.WSMessage;
using MySqlConnector;  

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Licencia QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;

        // Configuración Dinámica de CORS
        bool useTunnel = configuration.GetValue<bool>("ConnectionSettings:UseTunnel");
        string localFallback = "http://localhost:5168,http://localhost:5173";
        string corsOrigins = useTunnel 
            ? (configuration["ConnectionSettings:TunnelOrigin"] ?? localFallback)
            : (configuration["ConnectionSettings:LocalOrigin"] ?? localFallback);

        Console.WriteLine($"[CORS CONFIG] Usando origen: {corsOrigins}");

        var allowedOrigins = corsOrigins.Split(',')
            .Select(o => o.Trim())
            .Where(o => !string.IsNullOrEmpty(o))
            .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy("HappyPetsPolicy", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        //  Autenticación JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });

        //  Base de Datos - Construcción de cadena de conexión
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(dbHost) && !string.IsNullOrWhiteSpace(dbName) && !string.IsNullOrWhiteSpace(dbUser))
        {
            connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};Uid={dbUser};Pwd={dbPassword ?? string.Empty};";
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

       
        try
        {
            Console.WriteLine("[DB CHECK] Verificando disponibilidad del servidor MySQL...");
            
           
            using var testConnection = new MySqlConnection(connectionString);
            testConnection.Open(); 
            
            Console.WriteLine("[DB CHECK] Conexión exitosa a MySQL. Continuando con el arranque de la API.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("[ERROR CRÍTICO DE ARRANQUE] No se pudo establecer conexión con MySQL.");
            Console.WriteLine($"Detalle del error: {ex.Message}");
            Console.WriteLine("El arranque de la API ha sido abortado para prevenir fallos en cascada.");
            Console.WriteLine("==========================================================================\n");
            Console.ResetColor();

            throw new Exception("Arrancado cancelado: El servidor MySQL no responde.", ex);
        }
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

        //  Controladores y JSON Opciones
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        //  Swagger, SignalR y HealthChecks
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSignalR();
        services.AddHealthChecks();

        // Repositorios de la Aplicación
        services.AddScoped<ICitasRepository, CitasRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IAlertsRepository, AlertsRepository>();
        services.AddScoped<IClientPetRepository, ClientPetRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IConsultasRepository, ConsultasRepository>();
        services.AddScoped<IInvoiceExternalRepository, InvoiceExternalRepository>();
        services.AddScoped<IWSMRepository, WSMRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IFacturasRepository, FacturasRepository>();

        // Servicios de la Aplicación
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICreateProductService, ProductCreateService>();
        services.AddScoped<ICitasRequestService, CitasRequestService>();
        services.AddScoped<IConsultasService, ConsultasService>();
        services.AddScoped<IPetsService, PetsService>();
        services.AddScoped<IUserPetsService, UserPetsService>();
        services.AddScoped<ICreateCitaService, CreateCitaService>();
        services.AddScoped<IMessagingService, MessagingService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<GeneratePdfService>();
        services.AddScoped<IClientPetService, ClientPetService>();
        services.AddScoped<IReportSystemService, ReportSystemService>();
        services.AddScoped<GenerateReportExcel>();
        services.AddScoped<IMoneyTypeService, MoneyTypeService>();
        services.AddScoped<IBcvScraper, BcvScraper>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IWSMessage, WSMessage>();
        services.AddScoped<IInvoiceExternalService, InvoiceExternalService>();
        services.AddScoped<ICurrencyService, CurrencyService>();

        // Workers / Background Services
        services.AddHostedService<MessagePollerService>();
        services.AddHostedService<DeleteFacturaWorker>();
        services.AddHostedService<DeleteReportWorker>();
        services.AddHostedService<AutoGenerateReportWorker>();
        services.AddHostedService<BcvWorker>();
        services.AddHostedService<ClearNotificationsWorker>();

        // Clientes HTTP Externos (BCV)
        string bcvUrl = configuration["BcvSettings:ApiUrl"] 
            ?? throw new InvalidOperationException("La configuración 'BcvSettings:ApiUrl' no fue encontrada.");

        services.AddHttpClient("BcvClient", client =>
        {
            client.BaseAddress = new Uri(bcvUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
        });

        services.Configure<ApiSettingsOptions>(configuration.GetSection(ApiSettingsOptions.SectionName));
        services.Configure<TokenTemporalOptions>(configuration.GetSection(TokenTemporalOptions.SectionName));

        return services;
    }
}