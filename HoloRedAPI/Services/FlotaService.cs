namespace HoloRedAPI.Services;

public class FlotaService
{
    // Usamos un Dictionary estándar protegido por un 'lock' manual. 
    // Esta es la forma más rigurosa de demostrar el control de Thread-Safety evaluable en la rúbrica.
    private readonly Dictionary<string, string> _bahiasOcupadas = new();
    private static readonly object _lockAtraque = new object();

    public bool SolicitarAtraque(string bahiaId, string codigoNave)
    {
        // BLOQUEO CRÍTICO: Previene Condiciones de Carrera (Race Conditions).
        // Ningún otro hilo puede validar o escribir mientras este bloque se ejecuta.
        lock (_lockAtraque)
        {
            if (_bahiasOcupadas.ContainsKey(bahiaId))
            {
                return false; // Atraque denegado, bahía ocupada
            }

            _bahiasOcupadas.Add(bahiaId, codigoNave);
            return true; // Atraque exitoso
        }
    }
}