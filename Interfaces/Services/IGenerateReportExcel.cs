using vet_api_Net.Models;

namespace vet_api_Net.Interfaces.Services
{
    public interface IGenerateReportExcel
    {
        byte[] GenerateExcelFromReport(Reporte reporte);
    }
}
