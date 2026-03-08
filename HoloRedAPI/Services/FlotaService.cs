// Services/FlotaService.cs
using System.Collections.Concurrent;

public class FlotaService
{
    // Diccionario concurrente para saber si una bahía está ocupada
    private readonly ConcurrentDictionary<string, bool> _bahiasOcupadas = new();

    // Objeto de bloqueo (lock) para evitar Condiciones de Carrera
    private static readonly object _lockAtraque = new object();

    public bool SolicitarAtraque(string bahiaId, string codigoNave)
    {
        // BLOQUEO CRÍTICO: Solo un hilo (petición) entra aquí a la vez.
        lock (_lockAtraque)
        {
            // Verificamos si la bahía ya está ocupada
            if (_bahiasOcupadas.ContainsKey(bahiaId) && _bahiasOcupadas[bahiaId])
            {
                return false; // Atraque denegado, bahía ocupada
            }

            // Simulamos el tiempo de asignación y registro (Thread-Safe)
            _bahiasOcupadas[bahiaId] = true;
            return true; // Atraque exitoso
        }
    }
}