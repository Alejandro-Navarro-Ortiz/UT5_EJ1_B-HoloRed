// Controllers/InteligenciaController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class InteligenciaController : ControllerBase
{
    private readonly Neo4jRepository _neo4jRepo;

    public InteligenciaController(Neo4jRepository neo4jRepo)
    {
        _neo4jRepo = neo4jRepo;
    }

    [HttpGet("{faccion}/traidores")]
    public async Task<IActionResult> GetTraidores(string faccion)
    {
        try
        {
            var traidores = await _neo4jRepo.ObtenerTraidoresAsync(faccion);
            return Ok(traidores);
        }
        catch (Exception ex) when (ex.Message == "ERROR_RED_NEO4J")
        {
            // Código 503: Base de datos derribada, pero la API sigue en pie.
            return StatusCode(503, new { mensaje = "Interferencias en la HoloRed: Motor de Grafos inaccesible." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error crítico interno." });
        }
    }
}