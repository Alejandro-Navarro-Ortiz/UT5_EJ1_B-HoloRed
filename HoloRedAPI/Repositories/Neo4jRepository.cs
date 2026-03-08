// Repositories/Neo4jRepository.cs
using Neo4j.Driver;

public class Neo4jRepository
{
    private readonly IDriver _driver;

    public Neo4jRepository(IDriver driver) { _driver = driver; }

    public async Task<List<string>> ObtenerTraidoresAsync(string miFaccion)
    {
        try
        {
            await using var session = _driver.AsyncSession();
            var query = @"
                MATCH (f1:Faccion {nombre: $miFaccion})<-[:INFILTRADO_EN]-(e:Espia)-[:SUMINISTRA_ARMAS_A]->(p:Planeta)<-[:CONTROLA]-(f2:Faccion)
                WHERE f1 <> f2
                RETURN e.nombre AS Traidor";

            var result = await session.RunAsync(query, new { miFaccion });
            var traidores = new List<string>();
            await result.ForEachAsync(record => traidores.Add(record["Traidor"].As<string>()));
            return traidores;
        }
        catch (ServiceUnavailableException ex)
        {
            throw new Exception("ERROR_RED_NEO4J", ex);
        }
    }
}