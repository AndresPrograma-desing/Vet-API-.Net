
using System.Collections.Concurrent;
using vet_api_Net.Interfaze.Utilities;

//Describe:
// Servicio encargado de rastrear de forma centralizada y persistente los fallos acumulados en integraciones de terceros.
// Al usar ConcurrentDictionary y registrarse como Singleton, es seguro para hilos y mantiene el estado global.

namespace vet_api_Net.Utilities;

public class FailureTrackerUtilities : IFailureTrackerUtilities
{
    private readonly ConcurrentDictionary<string, (int Failures, int MaxLimit)> _failuresMap = new();

    public bool IsBlocked(string serviceKey)
    {
        if (_failuresMap.TryGetValue(serviceKey, out var state))
        {
            return state.Failures >= state.MaxLimit;
        }
        return false;
    }

    public void RecordFailure(string serviceKey, int maxFailures = 3)
    {
        _failuresMap.AddOrUpdate(serviceKey, 
            (Failures: 1, MaxLimit: maxFailures), 
            (key, current) => (Failures: current.Failures + 1, MaxLimit: maxFailures));
    }

    public void Reset(string serviceKey)
    {
        _failuresMap.TryRemove(serviceKey, out _);
    }
}
