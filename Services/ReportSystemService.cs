using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Interfaze.Repositories;
using vet_api_Net.Interfaze.Services;
using vet_api_Net.Models;
using vet_api_Net.Constants;

namespace vet_api_Net.Services
{
	public class ReportSystemService : IReportSystemService
	{
		private readonly IReportRepository _repository;
		private readonly IWorkerConfigRepository _workerConfigRepository;

		public ReportSystemService(IReportRepository repository, IWorkerConfigRepository workerConfigRepository)
		{
			_repository = repository;
			_workerConfigRepository = workerConfigRepository;
		}

		public async Task<IEnumerable<Reporte>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<Reporte?> GetByIdAsync(int id)
		{
			return await _repository.GetByIdAsync(id);
		}

		public async Task<Reporte> CreateAsync(Reporte reporte)
		{
			await _repository.AddAsync(reporte);
			await _repository.SaveChangesAsync();
			return reporte;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var reporte = await _repository.GetByIdTrackedAsync(id);
			if (reporte == null)
			{
				return false;
			}

			_repository.Remove(reporte);
			await _repository.SaveChangesAsync();
			return true;
		}

		public async Task<Reporte> GenerateFullSystemReportAsync(string generadoPor)
		{
			var (clientes, mascotas, productos, facturas, usuarios) = await _repository.GetSystemSnapshotAsync();

			var data = new
			{
				FechaGeneracion = DateTime.UtcNow,
				Clientes = clientes,
				Mascotas = mascotas,
				Productos = productos,
				Facturas = facturas,
				Usuarios = usuarios
			};

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

			await _repository.AddAsync(reporte);
			await _repository.SaveChangesAsync();
			return reporte;
		}
		
		public async Task<object?> IsEnabledAsync()
		{
			var deleteConfig = await _workerConfigRepository.GetByWorkerNameAsync(WorkerNames.DeleteReportWorker);
			var generateConfig = await _workerConfigRepository.GetByWorkerNameAsync(WorkerNames.AutoGenerateReportWorker);

			if (deleteConfig == null && generateConfig == null) return null;

			return new
			{
				Days = deleteConfig?.RetentionValue ?? 30,
				IsEnabled = deleteConfig?.IsEnabled ?? true,
				GenerateEnabled = generateConfig?.GenerateEnabled ?? false
			};
		}

		public async Task UpdateRetentionDaysAsync(int days)
		{
			var setting = await _workerConfigRepository.GetByWorkerNameAsync(WorkerNames.DeleteReportWorker);

			if (setting == null)
			{
				setting = new WorkerConfig { WorkerName = WorkerNames.DeleteReportWorker, RetentionValue = days };
				_workerConfigRepository.AddWorkerConfig(setting);
			}
			else
			{
				setting.RetentionValue = days;
				setting.LastUpdated = DateTime.UtcNow;
			}

			await _workerConfigRepository.SaveChangesAsync();
		}

		public async Task SetAutoDeleteEnabledAsync(bool enable)
		{
			var setting = await _workerConfigRepository.GetByWorkerNameAsync(WorkerNames.DeleteReportWorker);

			if (setting == null)
			{
				setting = new WorkerConfig { WorkerName = WorkerNames.DeleteReportWorker, IsEnabled = enable, RetentionValue = 30 };
				_workerConfigRepository.AddWorkerConfig(setting);
			}
			else
			{
				setting.IsEnabled = enable;
				setting.LastUpdated = DateTime.UtcNow;
			}

			await _workerConfigRepository.SaveChangesAsync();
		}

		public async Task SetAutoGenerateEnabledAsync(bool enable)
		{
			var setting = await _workerConfigRepository.GetByWorkerNameAsync(WorkerNames.AutoGenerateReportWorker);

			if (setting == null)
			{
				setting = new WorkerConfig { WorkerName = WorkerNames.AutoGenerateReportWorker, GenerateEnabled = enable };
				_workerConfigRepository.AddWorkerConfig(setting);
			}
			else
			{
				setting.GenerateEnabled = enable;
				setting.LastUpdated = DateTime.UtcNow;
			}

			await _workerConfigRepository.SaveChangesAsync();
		}
	}
}
