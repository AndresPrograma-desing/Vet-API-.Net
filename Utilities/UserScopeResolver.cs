using System;
using vet_api_Net.Constants;
using vet_api_Net.Interfaze.Utilities;

//Describe: Utilidad transversal que resuelve el alcance (doctorId/secretariaId) que un usuario autenticado puede consultar segun su rol, evitando que un doctor o secretaria accedan a citas/consultas que no le pertenecen. Un admin puede ver todo o filtrar por un doctor/secretaria especifico.
namespace vet_api_Net.Utilities;

public class UserScopeResolver : IUserScopeResolver
{
    public (int? DoctorId, int? SecretariaId) ResolveDoctorSecretariaScope(string? role, int? currentUserId, int? requestedDoctorId, int? requestedSecretariaId)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return (requestedDoctorId, requestedSecretariaId);
        }

        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException(ResponseErrors.Unauthorized);
        }

        if (role == Roles.Doctor)
        {
            return (currentUserId, null);
        }

        if (role == Roles.Secretaria)
        {
            return (null, currentUserId);
        }

        if (role == Roles.Admin)
        {
            return (requestedDoctorId, requestedSecretariaId);
        }

        throw new UnauthorizedAccessException(ResponseErrors.Forbidden);
    }
}
