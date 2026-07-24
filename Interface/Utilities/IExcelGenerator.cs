using DTOs;

namespace vet_api_Net.Interfaze.Utilities
{
    public interface IExcelGenerator
    {
        byte[] GenerateDashboardExcel(DashboardStatsDTO stats);
        byte[] GenerateCitasExcel(System.Collections.Generic.List<CitasRequestDTO> citas);
        byte[] GenerateClientesExcel(System.Collections.Generic.List<ClientListWithMascotasDTO> clientes);
    }
}
