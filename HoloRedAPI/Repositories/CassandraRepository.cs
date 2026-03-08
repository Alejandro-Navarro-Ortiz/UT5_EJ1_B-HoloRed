// Repositories/CassandraRepository.cs
using Cassandra;

public class CassandraRepository
{
    private readonly Cassandra.ISession _session;

    public CassandraRepository(Cassandra.ISession session) { _session = session; }

    public async Task InsertarImpactoAsync(string sectorId, DateTime fecha, string atacante, string objetivo, int dano)
    {
        try
        {
            var statement = new SimpleStatement(
                "INSERT INTO holored.impactos (SectorId, Fecha, Timestamp, NaveAtacante, NaveObjetivo, DanoEscudos) VALUES (?, ?, ?, ?, ?, ?)",
                sectorId, new LocalDate(fecha.Year, fecha.Month, fecha.Day), DateTime.UtcNow, atacante, objetivo, dano);
            await _session.ExecuteAsync(statement);
        }
        catch (NoHostAvailableException ex)
        {
            throw new Exception("ERROR_RED_CASSANDRA", ex);
        }
    }
    public async Task<List<object>> ObtenerHistorialAsync(string sectorId, DateTime fecha)
    {
        try
        {
            // Gracias a que la Clave de Partición es (SectorId, Fecha), esta consulta es hiperveloz y no hace Full Scan.
            var query = "SELECT Timestamp, NaveAtacante, NaveObjetivo, DanoEscudos FROM holored.impactos WHERE SectorId = ? AND Fecha = ?";
            var statement = new SimpleStatement(query, sectorId, new LocalDate(fecha.Year, fecha.Month, fecha.Day));

            var rs = await _session.ExecuteAsync(statement);

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
            throw new Exception("ERROR_RED_CASSANDRA", ex);
        }
    }
}