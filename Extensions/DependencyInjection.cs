using System;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using Npgsql;
using QuestPDF.Infrastructure;
using vet_api_Net.Data;
using vet_api_Net.HttpServices;
using vet_api_Net.Infrastructure.Configuration;
using vet_api_Net.Interface.Services;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Repositories.Clients;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Interfaze.Services.Clients;
using vet_api_Net.Interfaze.Utilities;
using vet_api_Net.Repositories;
using vet_api_Net.Services;
using vet_api_Net.Services.Supabase;
using vet_api_Net.Services.WSMessage;
using vet_api_Net.Utilities;
using vet_api_Net.Utilities.Pdf;
using vet_api_Net.Worker;
using vet_api_Net.Workers;
using vet_api_Net.Interfaze.Security;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    private static string? _cachedDbUrl = null;
    private static DateTime _lastChecked = DateTime.MinValue;
    private static readonly object _cacheLock = new object();

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Licencia QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;

        //  Base de Datos - Configuración Dual (MySQL / PostgreSQL)
        string dbProvider = configuration["DatabaseSettings:Provider"] ?? "MySQL";
        bool isPostgres = dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase);

        var connectionString = isPostgres
            ? configuration.GetConnectionString("SupabaseConnection")
            : configuration.GetConnectionString("DefaultConnection");

        if (!isPostgres)
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";

            if (!string.IsNullOrWhiteSpace(dbHost) && !string.IsNullOrWhiteSpace(dbName) && !string.IsNullOrWhiteSpace(dbUser))
            {
                connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};Uid={dbUser};Pwd={dbPassword ?? string.Empty};";
            }
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"Connection string for {dbProvider} is not configured.");
        }

        // Configuración Dinámica de CORS desde la Base de Datos
        // string localFallback = "http://localhost:5168,http://localhost:5173";
        string dbFrontendUrl = "";

        try
        {
            Console.WriteLine($"[DB CHECK] Verificando disponibilidad del servidor {dbProvider}...");
            using DbConnection testConnection = isPostgres
                ? new NpgsqlConnection(connectionString)
                : new MySqlConnection(connectionString);

            testConnection.Open();

            Console.WriteLine($"[DB CHECK] Conexión exitosa a {dbProvider}. Intentando leer configuraciones...");
            try
            {
                using DbCommand cmd = testConnection.CreateCommand();
                cmd.CommandText = "SELECT frontend_url FROM system_configs LIMIT 1";
                using var reader = cmd.ExecuteReader();
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    dbFrontendUrl = reader.GetString(0);
                    lock (_cacheLock)
                    {
                        _cachedDbUrl = dbFrontendUrl;
                        _lastChecked = DateTime.UtcNow;
                    }
                    Console.WriteLine($"[CORS CONFIG] URL leída de la base de datos: {dbFrontendUrl}");
                }
            }
            catch (Exception exQuery)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[CORS CONFIG WARNING] No se pudo leer SystemConfigs (tal vez falte la migración): {exQuery.Message}");
                Console.ResetColor();
            }

            Console.WriteLine($"[DB CHECK] Continuando con el arranque de la API.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n==========================================================================");
            Console.WriteLine($"[ERROR CRÍTICO DE ARRANQUE] No se pudo establecer conexión con {dbProvider}.");
            Console.WriteLine($"Detalle del error: {ex.Message}");
            Console.WriteLine("El arranque de la API ha sido abortado para prevenir fallos en cascada.");
            Console.WriteLine("==========================================================================\n");
            Console.ResetColor();

            throw new Exception($"Arrancado cancelado: El servidor {dbProvider} no responde.", ex);
        }

        services.AddCors(options =>
        {
            options.AddPolicy("HappyPetsPolicy", policy =>
            {
                policy.SetIsOriginAllowed(origin =>
                {
                    // 1. Permitir localhost siempre
                    if (origin.StartsWith("http://localhost:") || origin.StartsWith("https://localhost:")) return true;

                    // 2. Permitir túnel por defecto de appsettings
                    string tunnelOrigin = configuration["ConnectionSettings:TunnelOrigin"] ?? "";
                    if (!string.IsNullOrEmpty(tunnelOrigin) && origin.Equals(tunnelOrigin, StringComparison.OrdinalIgnoreCase)) return true;

                    // 3. Consultar Base de Datos con caché de 30 segundos
                    lock (_cacheLock)
                    {
                        if (DateTime.UtcNow - _lastChecked > TimeSpan.FromSeconds(30))
                        {
                            try
                            {
                                using DbConnection conn = isPostgres
                                    ? new NpgsqlConnection(connectionString)
                                    : new MySqlConnection(connectionString);
                                conn.Open();
                                using DbCommand cmd = conn.CreateCommand();
                                cmd.CommandText = "SELECT frontend_url FROM system_configs LIMIT 1";
                                var val = cmd.ExecuteScalar();
                                _cachedDbUrl = val != DBNull.Value ? val?.ToString() : null;
                                _lastChecked = DateTime.UtcNow;
                                Console.WriteLine($"[CORS DYNAMIC] URL actualizada desde BD: {_cachedDbUrl}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[CORS DYNAMIC WARNING] Fallo al consultar BD para CORS: {ex.Message}");
                            }
                        }

                        if (!string.IsNullOrEmpty(_cachedDbUrl))
                        {
                            var allowedFromDb = _cachedDbUrl.Split(',')
                                .Select(o => o.Trim().TrimEnd('/'));
                            if (allowedFromDb.Contains(origin.TrimEnd('/'), StringComparer.OrdinalIgnoreCase)) return true;
                        }
                    }

                    return false;
                })
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

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        var rateLimitingOptions = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>()
            ?? new RateLimitingOptions();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var path = httpContext.Request.Path.Value ?? string.Empty;
                if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/hubs/", StringComparison.OrdinalIgnoreCase))
                {
                    return RateLimitPartition.GetNoLimiter("exempt");
                }

                var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitingOptions.GlobalPermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimitingOptions.GlobalWindowSeconds),
                    QueueLimit = 0
                });
            });

            options.AddPolicy("auth", httpContext =>
            {
                var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitingOptions.AuthPermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimitingOptions.AuthWindowSeconds),
                    QueueLimit = 0
                });
            });
        });

        // (El chequeo de DB y la cadena de conexión se han movido arriba de CORS)

        services.AddDbContext<AppDbContext>(options =>
        {
            if (isPostgres)
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        });

        //  Controladores y JSON Opciones
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        //  Swagger, SignalR y HealthChecks
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Vet API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Ingresa el token JWT."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        services.AddSignalR();
        services.AddHealthChecks();

        // Repositorios de la Aplicación
        services.AddScoped<ICitasRepository, CitasRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IAlertsRepository, AlertsRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IClientPetRepository, ClientPetRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IConsultasRepository, ConsultasRepository>();
        services.AddScoped<IInvoiceExternalRepository, InvoiceExternalRepository>();
        services.AddScoped<IWSMRepository, WSMRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IFacturasRepository, FacturasRepository>();
        services.AddScoped<IPasswordRecoveryRepository, PasswordRecoveryRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<ISystemConfigRepository, SystemConfigRespository>();
        services.AddScoped<IHistoriaClinicaRepository, HistoriaClinicaRepository>();
        services.AddScoped<IIaConocimientoRepository, IaConocimientoRepository>();
        services.AddScoped<IPasswordResetTicketRepository, PasswordResetTicketRepository>();
        services.AddScoped<ICalendarRepository, CalendarRepository>();
        services.AddScoped<ILogsSistemaRepository, LogsSistemaRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
        services.AddScoped<IVaccineRepository, VaccineRepository>();
        services.AddScoped<IEspecieRepository, EspecieRepository>();
        services.AddScoped<IPetVaccinationRepository, PetVaccinationRepository>();
        services.AddScoped<IWorkerConfigRepository, WorkerConfigRepository>();

        // Servicios de la Aplicación
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();
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
        services.AddScoped<GeneratePdfUtilities>();
        services.AddScoped<IClientPetService, ClientPetService>();
        services.AddScoped<IReportSystemService, ReportSystemService>();
        services.AddScoped<GenerateReportExcel>();
        services.AddScoped<IMoneyTypeService, MoneyTypeService>();
        services.AddScoped<IBcvScraper, BcvScraper>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationsPushService, NotificationsPushService>();
        services.AddScoped<IWSMessage, WSMessage>();
        services.AddScoped<IInvoiceExternalService, InvoiceExternalService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISupabaseService, SupabaseService>();
        services.AddScoped<IHistoriaClinicaService, HistoriaClinicaService>();
        services.AddScoped<INowGrodService, NowGrodService>();
        services.AddScoped<IIaConocimientoService, IaConocimientoService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<ISystemLogService, SystemLogService>();
        services.AddScoped<IVaccineService, VaccineService>();
        services.AddScoped<IEspecieService, EspecieService>();
        services.AddScoped<IPetVaccinationService, PetVaccinationService>();
        services.AddScoped<IWorkerConfigService, WorkerConfigService>();
        services.AddHttpClient<IEmailSenderService, vet_api_Net.HttpServices.ResendEmailService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });

        //Utilities
        services.AddSingleton<IFailureTrackerUtilities, FailureTrackerUtilities>();
        services.AddScoped<IHistoriaClinicaAnalizadorUtilities, HistoriaClinicaAnalizadorUtilities>();
        services.AddScoped<ICurrencyUtilities, CurrencyUtilities>();
        services.AddScoped<IExcelGeneratorUtilities, ExcelGeneratorUtilities>();
        services.AddScoped<IConsultaPdfUtilities, ConsultaPdfUtilities>();
        services.AddScoped<IImageConversionUtilities, ImageConversionUtilities>();
        services.AddHttpClient<IProfilePdfUtilities, ProfilePdfUtilities>();
        services.AddScoped<IVaccinationCertificatePdfUtilities, VacunaCertifyPdfUtilities>();
        services.AddScoped<IUserScopeResolver, UserScopeResolver>();

        // Seguridad de datos
        services.AddScoped<ILoginSecurity, LoginSecurity>();
        
        // Workers / Background Services
        services.AddHostedService<MessagePollerService>();
        services.AddHostedService<DeleteFacturaWorker>();
        services.AddHostedService<DeleteReportWorker>();
        services.AddHostedService<AutoGenerateReportWorker>();
        services.AddHostedService<BcvWorker>();
        services.AddHostedService<ClearNotificationsWorker>();
        services.AddHostedService<VaccinationReminderWorker>();

        // Clientes HTTP Externos (BCV)
        services.AddHttpClient("BcvClient", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
        });
        //Configuraciones de ApiSettingsOptions
        services.Configure<ApiSettingsOptions>(configuration.GetSection(ApiSettingsOptions.SectionName));
        services.Configure<TokenTemporalOptions>(configuration.GetSection(TokenTemporalOptions.SectionName));
        services.Configure<TemplatesHTML>(configuration.GetSection(TemplatesHTML.SectionName));
        services.Configure<SecurityOptions>(configuration.GetSection(SecurityOptions.SectionName));
        services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));

        return services;
    }
}