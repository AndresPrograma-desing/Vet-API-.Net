using DTOs;

namespace vet_api_Net.Interfaze.Utilities;

public interface IProfilePdfUtilities
{
    byte[] GenerateCredencialPdf(CredencialUsuarioPdfDTO usuario, string webRootPath);
    string BuildFileName(CredencialUsuarioPdfDTO usuario);
}
