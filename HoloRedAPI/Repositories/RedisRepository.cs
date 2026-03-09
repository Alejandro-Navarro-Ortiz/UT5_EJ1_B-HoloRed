using StackExchange.Redis;
using HoloRedAPI.Exceptions;

namespace HoloRedAPI.Repositories;

/// <summary>
/// Repositorio para la interacción con Redis (Almacén de estructuras de datos en memoria).
/// Orientado al almacenamiento en caché (caching) y estados con tiempo de vida limitado (TTL).
/// </summary>
public class RedisRepository
{
    private readonly IDatabase _db;

    public RedisRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    /// <summary>
    /// Actualiza el estado de la baliza de una nave con un tiempo de expiración.
    /// </summary>
    public async Task ActualizarEstadoNaveAsync(string codigoNave, string estado)
    {
        try
        {
            string clave = $"nave:{codigoNave}:estado";

            // Escritura en RAM atómica y ultrarrápida. 
            // Incorpora un TTL (Time To Live) estricto de 10 minutos (se borrará solo después).
            await _db.StringSetAsync(clave, estado, TimeSpan.FromMinutes(10));
        }
        catch (RedisConnectionException ex)
        {
            throw new DatabaseOfflineException("Radar fuera de línea (Redis caído)", ex);
        }
    }

    /// <summary>
    /// Obtiene una ruta hiperespacial, priorizando recuperar el cálculo previo de la caché para ganar velocidad.
    /// </summary>
    public async Task<string> ObtenerRutaRapidaAsync(string origen, string destino)
    {
        try
        {
            string claveCache = $"ruta:{origen}:{destino}";

            // Intenta obtener la ruta previamente calculada en la caché
            var rutaEnCache = await _db.StringGetAsync(claveCache);

            if (rutaEnCache.HasValue)
            {
                // Si existe (HIT), devuelve el resultado inmediatamente
                return $"[CACHÉ] Ruta recuperada en submilisegundos: {rutaEnCache}";
            }

            // Si no existe (MISS), se simula un cálculo pesado o complejo de ruta
            string nuevaRuta = $"Salto óptimo trazado por el corredor de {origen.Substring(0, 3)}-X hacia {destino}.";

            // Persistencia efímera: Guarda este nuevo resultado en la caché con un TTL de 30 minutos
            // para que las próximas peticiones lo encuentren rápido.
            await _db.StringSetAsync(claveCache, nuevaRuta, TimeSpan.FromMinutes(30));

            return $"[NUEVO CÁLCULO] {nuevaRuta}";
        }
        catch (RedisConnectionException ex)
        {
            throw new DatabaseOfflineException("Radar fuera de línea (Redis caído)", ex);
        }
    }
}