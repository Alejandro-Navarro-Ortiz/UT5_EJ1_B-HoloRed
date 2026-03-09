using Microsoft.AspNetCore.Mvc;
using HoloRedAPI.Repositories;
using HoloRedAPI.Models;
using HoloRedAPI.Exceptions;

namespace HoloRedAPI.Controllers;

/// <summary>
/// Controlador destinado al almacenamiento de grandes volúmenes de datos.
/// Gestiona telemetría y registros de impactos usando Cassandra.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TelemetriaController : ControllerBase
{
    private readonly CassandraRepository _cassandraRepo;

    public TelemetriaController(CassandraRepository cassandraRepo)
    {
        _cassandraRepo = cassandraRepo;
    }

    /// <summary>
    /// Endpoint POST para registrar un nuevo impacto de combate en un sector.
    /// Diseñado para operaciones de escritura masivas.
    /// </summary>
    [HttpPost("impacto")]
    public async Task<IActionResult> RegistrarImpacto([FromBody] ImpactoRequest r)
    {
        try
        {
            await _cassandraRepo.InsertarImpactoAsync(r.SectorId, r.Fecha, r.Atacante, r.Objetivo, r.Dano);
            return Ok(new { mensaje = "Impacto registrado masivamente en la matriz columnar." });
        }
        catch (DatabaseOfflineException ex)
        {
            return StatusCode(503, new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Endpoint GET para recuperar el historial de impactos en un sector y fecha dados.
    /// </summary>
    [HttpGet("historial/{sector}")]
    public async Task<IActionResult> GetHistorial(string sector, [FromQuery] DateTime fecha)
    {
        try
        {
            var historial = await _cassandraRepo.ObtenerHistorialAsync(sector, fecha);
            return Ok(historial);
        }
        catch (DatabaseOfflineException ex)
        {
            return StatusCode(503, new { mensaje = ex.Message });
        }
    }
}