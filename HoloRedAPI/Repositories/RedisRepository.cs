// Repositories/RedisRepository.cs
using StackExchange.Redis;

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
            // Asignamos el estado y un TTL de 10 minutos atómicamente
            await _db.StringSetAsync(clave, estado, TimeSpan.FromMinutes(10));
        }
        catch (RedisConnectionException ex)
        {
            throw new Exception("ERROR_RED_REDIS", ex);
        }
    }
}