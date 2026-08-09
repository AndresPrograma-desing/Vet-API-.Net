using DTOs;

namespace vet_api_Net.Interfaze.Utilities;

public interface IConsultaPdfUtilities
{
    byte[] GenerateConsultaPdf(ConsultaPdfDTO consulta, string webRootPath, string currencySymbol);
    string BuildFileName(ConsultaPdfDTO consulta);
}
