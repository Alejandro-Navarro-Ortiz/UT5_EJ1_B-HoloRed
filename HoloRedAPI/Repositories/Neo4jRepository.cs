using Neo4j.Driver;
using HoloRedAPI.Exceptions;

namespace HoloRedAPI.Repositories;

/// <summary>
/// Repositorio para la interacción con la base de datos de grafos Neo4j.
/// Ideal para navegar a través de relaciones altamente conectadas (redes de espionaje).
/// </summary>
public class Neo4jRepository
{
    private readonly IDriver _driver;

    public Neo4jRepository(IDriver driver) { _driver = driver; }

    /// <summary>
    /// Consulta el grafo de inteligencia para descubrir traidores en una facción.
    /// </summary>
    public async Task<List<string>> ObtenerTraidoresAsync(string miFaccion)
    {
        try
        {
            await using var session = _driver.AsyncSession();

            // Consulta Cypher: Explora múltiples relaciones para detectar traidores.
            // Busca a un espía (e) infiltrado en tu facción (f1) que además suministra armas 
            // a un planeta controlado por otra facción distinta (f2).
            var query = @"
                MATCH (f1:Faccion {nombre: $miFaccion})<-[:INFILTRADO_EN]-(e:Espia)-[:SUMINISTRA_ARMAS_A]->(p:Planeta)<-[:CONTROLA]-(f2:Faccion)
                WHERE f1 <> f2
                RETURN e.nombre AS Traidor";

            var result = await session.RunAsync(query, new { miFaccion });
            var traidores = new List<string>();

            // Recorre los nodos de resultado y extrae el campo Traidor
            await result.ForEachAsync(record => traidores.Add(record["Traidor"].As<string>()));

            return traidores;
        }
        catch (ServiceUnavailableException ex)
        {
            throw new DatabaseOfflineException("Red de espionaje interceptada (Neo4j caído)", ex);
        }
    }
}