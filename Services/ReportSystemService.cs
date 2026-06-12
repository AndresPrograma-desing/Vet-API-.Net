using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Interfaces.Services;
using vet_api_Net.Interfaces.Repositories;
using vet_api_Net.Models;
using vet_api_Net.Constants;

namespace vet_api_Net.Services
{
    public class ReportSystemService : IReportSystemService
    {
        private readonly IReportRepository _reportRepository;

        public ReportSystemService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<IEnumerable<Reporte>> GetAllAsync()
        {
            return await _reportRepository.GetAllAsync();
        }

        public async Task<Reporte> GetByIdAsync(int id)
        {
            return await _reportRepository.GetByIdAsync(id);
        }

        public async Task<Reporte> CreateAsync(Reporte reporte)
        {
            await _reportRepository.AddAsync(reporte);
            await _reportRepository.SaveChangesAsync();
            return reporte;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var reporte = await _reportRepository.GetByIdAsync(id);
            if (reporte == null)
            {
                return false;
            }

            _reportRepository.Delete(reporte);
            await _reportRepository.SaveChangesAsync();
            return true;
        }

        public async Task<Reporte> GenerateFullSystemReportAsync(string generadoPor)
        {
            var data = await _reportRepository.GetSystemReportDataAsync();
            string jsonData = System.Text.Json.JsonSerializer.Serialize(data);

            var reporte = new Reporte
            {
                Titulo = ResponseMessagesReport.ReportTittle,
                FechaCreacion = DateTime.UtcNow,
                Categoria = ResponseMessagesReport.ResportCategory,
                Filtro = ResponseMessagesReport.Filtre,
                Datos = jsonData,
                GeneradoPor = string.IsNullOrWhiteSpace(generadoPor) ? "sistema" : generadoPor
            };

            await _reportRepository.AddAsync(reporte);
            await _reportRepository.SaveChangesAsync();
            return reporte;
        }

        public async Task<object?> IsEnabledAsync()
        {
            var config = await _reportRepository.GetConfigAsync();
            if (config == null) return null;

            return new
            {
                config.Days,
                config.IsEnabled,
                config.GenerateEnabled
            };
        }

        public async Task UpdateRetentionDaysAsync(int days)
        {
            var setting = await _reportRepository.GetConfigAsync();
            if (setting == null)
            {
                setting = new ReporConfig { Days = days };
                _reportRepository.AddConfig(setting);
            }
            else
            {
                setting.Days = days;
                setting.LastUpdated = DateTime.UtcNow;
                _reportRepository.UpdateConfig(setting);
            }

            await _reportRepository.SaveChangesAsync();
        }

        public async Task ToggleAutoDeleteAsync(bool enable)
        {
            var setting = await _reportRepository.GetConfigAsync();
            if (setting == null)
            {
                setting = new ReporConfig { IsEnabled = enable, Days = 30 };
                _reportRepository.AddConfig(setting);
            }
            else
            {
                setting.IsEnabled = enable;
                setting.LastUpdated = DateTime.UtcNow;
                _reportRepository.UpdateConfig(setting);
            }

            await _reportRepository.SaveChangesAsync();
        }

        public async Task ToggleAutoGenerateAsync(bool enable)
        {
            var setting = await _reportRepository.GetConfigAsync();
            if (setting == null)
            {
                setting = new ReporConfig { GenerateEnabled = enable, Days = 30 };
                _reportRepository.AddConfig(setting);
            }
            else
            {
                setting.GenerateEnabled = enable;
                setting.LastUpdated = DateTime.UtcNow;
                _reportRepository.UpdateConfig(setting);
            }

            await _reportRepository.SaveChangesAsync();
        }
    }
}
