using Cassandra;
using HoloRedAPI.Exceptions;

namespace HoloRedAPI.Repositories;

/// <summary>
/// Repositorio para la interacción con la base de datos columnar Apache Cassandra.
/// Orientado a operaciones masivas de inserción (telemetría y eventos).
/// </summary>
public class CassandraRepository
{
    private readonly ICluster _cluster;
    private Cassandra.ISession? _session;

    public CassandraRepository(ICluster cluster)
    {
        _cluster = cluster;
    }

    /// <summary>
    /// Patrón Lazy-Load: Resiliencia en el arranque. 
    /// Solo intenta conectar con la base de datos Cassandra en el momento de hacer la primera consulta.
    /// </summary>
    private Cassandra.ISession GetSession()
    {
        if (_session == null)
            _session = _cluster.Connect("holored");
        return _session;
    }

    /// <summary>
    /// Inserta un registro de impacto en la base de datos.
    /// </summary>
    public async Task InsertarImpactoAsync(string sectorId, DateTime fecha, string atacante, string objetivo, int dano)
    {
        try
        {
            var session = GetSession();

            // Inserción masiva (Append-only). Las bases de datos columnares como Cassandra
            // están optimizadas para un altísimo rendimiento de escritura.
            var statement = new SimpleStatement(
                "INSERT INTO impactos (SectorId, Fecha, Timestamp, NaveAtacante, NaveObjetivo, DanoEscudos) VALUES (?, ?, ?, ?, ?, ?)",
                sectorId, new LocalDate(fecha.Year, fecha.Month, fecha.Day), DateTime.UtcNow, atacante, objetivo, dano);

            await session.ExecuteAsync(statement);
        }
        catch (NoHostAvailableException ex)
        {
            throw new DatabaseOfflineException("Sistema de telemetría dañado (Cassandra caído)", ex);
        }
    }

    /// <summary>
    /// Recupera el historial de eventos ocurridos en un sector y fecha específicos.
    /// </summary>
    public async Task<List<object>> ObtenerHistorialAsync(string sectorId, DateTime fecha)
    {
        try
        {
            var session = GetSession();

            // Consulta optimizada: SectorId y Fecha componen la Partition Key (Clave de partición).
            // Esto garantiza un acceso directo al nodo correcto, evitando un Full Scan (escaneo completo).
            var query = "SELECT Timestamp, NaveAtacante, NaveObjetivo, DanoEscudos FROM impactos WHERE SectorId = ? AND Fecha = ?";
            var statement = new SimpleStatement(query, sectorId, new LocalDate(fecha.Year, fecha.Month, fecha.Day));

            var rs = await session.ExecuteAsync(statement);
            var historial = new List<object>();

            // Mapeo del resultado de Cassandra a un formato estándar
            foreach (var row in rs)
            {
                historial.Add(new
                {
                    Hora = row.GetValue<DateTime>("timestamp"),
                    Atacante = row.GetValue<string>("naveatacante"),
                    Objetivo = row.GetValue<string>("naveobjetivo"),
                    Dano = row.GetValue<int>("danoescudos")
                });
            }
            return historial;
        }
        catch (NoHostAvailableException ex)
        {
            throw new DatabaseOfflineException("Sistema de telemetría dañado (Cassandra caído)", ex);
        }
    }
}