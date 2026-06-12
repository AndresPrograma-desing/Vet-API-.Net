using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using DTOs;
using vet_api_Net.Data;
using vet_api_Net.Hubs;
using vet_api_Net.Constants;
using vet_api_Net.Interfaces.Repositories;

namespace vet_api_Net.Services
{
    public class MessagePollerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ILogger<MessagePollerService> _logger;
        private readonly ConcurrentDictionary<int, bool> _sentMessages = new();
        private bool? _mensajesTableExists;

        public MessagePollerService(IServiceScopeFactory scopeFactory, IHubContext<MessageHub> hubContext, ILogger<MessagePollerService> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var messagesRepository = scope.ServiceProvider.GetRequiredService<IMessagesRepository>();
 
                    if (_mensajesTableExists != true)
                    {
                        _mensajesTableExists = await MensajesTableExistsAsync(db, stoppingToken);
                        if (_mensajesTableExists != true)
                        { 
                            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                            continue;
                        }
                    }

                    var messages = await messagesRepository.GetUnreadMessagesAsync();

                    foreach (var m in messages)
                    {
                        if (_sentMessages.TryAdd(m.Id, true))
                        {
                            var dto = new MensajeDTO
                            {
                                Id = m.Id,
                                EmisorId = m.EmisorId,
                                ReceptorId = m.ReceptorId,
                                Contenido = m.Contenido,
                                Leido = m.Leido ?? false,
                                FechaEnvio = m.FechaEnvio
                            };

                            // Notify receptor and sender groups
                            await _hubContext.Clients.Group(m.ReceptorId.ToString()).SendAsync("ReceiveMessage", dto);
                            await _hubContext.Clients.Group(m.EmisorId.ToString()).SendAsync("ReceiveMessage", dto);
                        }
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("mensajes", StringComparison.OrdinalIgnoreCase)
                        && ex.Message.Contains("doesn't exist", StringComparison.OrdinalIgnoreCase))
                    {
                        _mensajesTableExists = false; 
                    }
                    else
                    {
                         
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(3600), stoppingToken);
            }
        }

        private static async Task<bool> MensajesTableExistsAsync(AppDbContext db, CancellationToken cancellationToken)
        {
            try
            {
                var conn = db.Database.GetDbConnection();
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync(cancellationToken);

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = MessagePoller.Query;
                var result = await cmd.ExecuteScalarAsync(cancellationToken);
                return result != null && result != DBNull.Value;
            }
            catch
            {
                return false;
            }
        }
    }
}
