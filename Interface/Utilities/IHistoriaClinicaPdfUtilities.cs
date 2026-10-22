using DTOs;

namespace vet_api_Net.Interfaze.Utilities;

public interface IHistoriaClinicaPdfUtilities
{
    byte[] GenerateHistoriaClinicaPdf(HistoriaClinicaResponseDTO historia, string webRootPath);
    string BuildFileName(HistoriaClinicaResponseDTO historia);
}
