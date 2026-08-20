using System.Collections.Generic;
using System.Threading.Tasks;
using vet_api_Net.Models;
using DTOs;

//Describe: Interfaz para el analizador clínico interno que genera el resumen, alertas y sugerencias de la mascota.
namespace vet_api_Net.Interfaze.Utilities;

public interface IHistoriaClinicaAnalizadorUtilities
{
    Task<ResumenClinicoIAResponseDTO> GenerarAnalisisClinicoAsync(Mascota mascota, List<Consulta> consultas, List<PetVaccination> vaccinations, string? resumenAnterior = null);
}
