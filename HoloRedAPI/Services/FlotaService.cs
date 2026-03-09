namespace HoloRedAPI.Services;

/// <summary>
/// Servicio de la Flota que contiene la lógica de negocio central sobre las naves y bahías.
/// Es responsable de gestionar la concurrencia de forma segura.
/// </summary>
public class FlotaService
{
    // Diccionario en memoria para registrar qué bahía está ocupada y por qué nave.
    // Usamos un Dictionary estándar pero lo protegeremos con un bloqueo manual (lock).
    private readonly Dictionary<string, string> _bahiasOcupadas = new();

    // Objeto de bloqueo estático/privado para sincronizar los diferentes hilos (requests).
    private static readonly object _lockAtraque = new object();

    /// <summary>
    /// Intenta registrar una nave en una bahía específica, asegurando que dos naves no ocupen 
    /// el mismo espacio al mismo tiempo debido a concurrencia.
    /// </summary>
    /// <returns>True si el atraque se concede, False si la bahía ya estaba ocupada.</returns>
    public bool SolicitarAtraque(string bahiaId, string codigoNave)
    {
        // BLOQUEO CRÍTICO (Thread-Safety): Previene Condiciones de Carrera (Race Conditions).
        // Ningún otro hilo (petición web simultánea) puede validar o escribir 
        // mientras este bloque se está ejecutando. Garantiza que la lectura y la inserción
        // ocurran de manera atómica.
        lock (_lockAtraque)
        {
            // Comprueba si la bahía ya existe en el diccionario
            if (_bahiasOcupadas.ContainsKey(bahiaId))
            {
                return false; // Atraque denegado, bahía ocupada
            }

            // Si no está ocupada, se añade al diccionario
            _bahiasOcupadas.Add(bahiaId, codigoNave);
            return true; // Atraque exitoso
        }
    }
}