namespace vet_api_Net.Interfaze.Utilities;

public interface IUserScopeResolver
{
    (int? DoctorId, int? SecretariaId) ResolveDoctorSecretariaScope(string? role, int? currentUserId, int? requestedDoctorId, int? requestedSecretariaId);
}
