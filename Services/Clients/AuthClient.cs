// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using DTOs;
// using vet_api_Net.Models;
// using vet_api_Net.Constants;
// using vet_api_Net.Interfaze.Repositories.Clients;
// using vet_api_Net.Interfaze.Services.Clients;

// namespace vet_api_Net.Services;

// public class AuthClients(IClientRepository clientRepository) : IAuthClient
// {
//     private readonly IClientRepository _clientRepository = clientRepository;

//     public async Task<AuthClientsDTO> GetValidateClientAsync(string email, int cedula)
//     {
//         var client = await _clientRepository.GetByIdSimpleAsync()
//     }
// }