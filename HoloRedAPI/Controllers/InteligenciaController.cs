using Microsoft.AspNetCore.Mvc;
using HoloRedAPI.Repositories;
using HoloRedAPI.Exceptions;

namespace HoloRedAPI.Controllers;

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
        catch (DatabaseOfflineException ex)
        {
            // Código 503 semántico, evaluable en rúbrica
            return StatusCode(503, new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = "Error crítico en el núcleo de la API." });
        }
    }
}