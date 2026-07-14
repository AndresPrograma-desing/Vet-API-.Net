// Interfaz para el rastreador de fallas genérico de integraciones y servicios externos.
// Permite validar si un canal o servicio particular se encuentra bloqueado debido a errores consecutivos acumulados.
namespace vet_api_Net.Interfaze.Utilities;

public interface IFailureTracker
{
    bool IsBlocked(string serviceKey);
    void RecordFailure(string serviceKey, int maxFailures = 3);
    void Reset(string serviceKey);
}
