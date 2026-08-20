using DTOs;

namespace vet_api_Net.Interfaze.Utilities;

public interface IVaccinationCertificatePdfUtilities
{
    byte[] GenerateCarnetPdf(PetVaccinationCarnetPdfDTO carnet, string webRootPath);
    string BuildFileName(PetVaccinationCarnetPdfDTO carnet);
}
