using StackExchange.Redis;
using HoloRedAPI.Exceptions;

namespace HoloRedAPI.Repositories;

public class RedisRepository
{
    private readonly IDatabase _db;

    public RedisRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task ActualizarEstadoNaveAsync(string codigoNave, string estado)
    {
        try
        {
            string clave = $"nave:{codigoNave}:estado";
            // Escritura en RAM atómica con TTL estricto de 10 minutos
            await _db.StringSetAsync(clave, estado, TimeSpan.FromMinutes(10));
        }
        catch (RedisConnectionException ex)
        {
            throw new DatabaseOfflineException("Radar fuera de línea (Redis caído)", ex);
        }
    }

    public async Task<string> ObtenerRutaRapidaAsync(string origen, string destino)
    {
        try
        {
            string claveCache = $"ruta:{origen}:{destino}";
            var rutaEnCache = await _db.StringGetAsync(claveCache);

            if (rutaEnCache.HasValue)
            {
                return $"[CACHÉ] Ruta recuperada en submilisegundos: {rutaEnCache}";
            }

            // Simulación de cálculo pesado si no existe en la caché
            string nuevaRuta = $"Salto óptimo trazado por el corredor de {origen.Substring(0, 3)}-X hacia {destino}.";

            // Persistencia efímera: TTL de 30 minutos para la ruta
            await _db.StringSetAsync(claveCache, nuevaRuta, TimeSpan.FromMinutes(30));

            return $"[NUEVO CÁLCULO] {nuevaRuta}";
        }
        catch (RedisConnectionException ex)
        {
            throw new DatabaseOfflineException("Radar fuera de línea (Redis caído)", ex);
        }
    }
}