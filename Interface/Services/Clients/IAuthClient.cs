using System;
using DTOs;

namespace vet_api_Net.Interfaze.Services.Clients;

public interface IAuthClient
{
    Task<AuthClientsDTO> GetValidateClientAsync(string email, int cedula);
}