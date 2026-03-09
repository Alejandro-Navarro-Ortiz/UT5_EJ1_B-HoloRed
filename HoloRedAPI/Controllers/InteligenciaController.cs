using Microsoft.AspNetCore.Mvc;
using HoloRedAPI.Repositories;
using HoloRedAPI.Exceptions;

namespace HoloRedAPI.Controllers;

/// <summary>
/// Controlador encargado de gestionar las operaciones de inteligencia y espionaje.
/// Se conecta a la base de datos orientada a grafos (Neo4j).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InteligenciaController : ControllerBase
{
    private readonly Neo4jRepository _neo4jRepo;

    /// <summary>
    /// Constructor que inyecta el repositorio de Neo4j.
    /// </summary>
    public InteligenciaController(Neo4jRepository neo4jRepo)
    {
        _neo4jRepo = neo4jRepo;
    }

    /// <summary>
    /// Endpoint GET para buscar y listar los posibles traidores de una facción específica.
    /// </summary>
    /// <param name="faccion">Nombre de la facción sobre la que se quiere consultar.</param>
    /// <returns>Lista de traidores o mensajes de error semánticos correspondientes.</returns>
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
            // Código 503 (Service Unavailable) si la base de datos Neo4j está caída
            return StatusCode(503, new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            // Código 500 (Internal Server Error) para errores imprevistos
            return StatusCode(500, new { mensaje = "Error crítico en el núcleo de la API." });
        }
    }
}