using Cassandra;
using HoloRedAPI.Exceptions;

namespace HoloRedAPI.Repositories;

public class CassandraRepository
{
    private readonly ICluster _cluster;
    private Cassandra.ISession? _session;

    public CassandraRepository(ICluster cluster)
    {
        _cluster = cluster;
    }

    // Patrón Lazy-Load: Resiliencia en el arranque. Solo intenta conectar si hay un impacto real.
    private Cassandra.ISession GetSession()
    {
        if (_session == null)
            _session = _cluster.Connect("holored");
        return _session;
    }

    public async Task InsertarImpactoAsync(string sectorId, DateTime fecha, string atacante, string objetivo, int dano)
    {
        try
        {
            var session = GetSession();
            // Inserción masiva (Append-only). Diseñado para altísimo rendimiento de escritura.
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

    public async Task<List<object>> ObtenerHistorialAsync(string sectorId, DateTime fecha)
    {
        try
        {
            var session = GetSession();
            // Consulta optimizada: SectorId y Fecha componen la Partition Key. 
            // Esto garantiza que NO haya un Full Scan en la base de datos distribuida.
            var query = "SELECT Timestamp, NaveAtacante, NaveObjetivo, DanoEscudos FROM impactos WHERE SectorId = ? AND Fecha = ?";
            var statement = new SimpleStatement(query, sectorId, new LocalDate(fecha.Year, fecha.Month, fecha.Day));

            var rs = await session.ExecuteAsync(statement);
            var historial = new List<object>();

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