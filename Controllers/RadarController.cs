using Microsoft.AspNetCore.Mvc;
using HoloRedAPI.Repositories;
using HoloRedAPI.Models;
using HoloRedAPI.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace HoloRedAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RadarController : ControllerBase
{
    private readonly RedisRepository _redisRepo;

    public RadarController(RedisRepository redisRepo)
    {
        _redisRepo = redisRepo;
    }

    [HttpPost("baliza/{codigo_nave}")]
    public async Task<IActionResult> ActualizarBaliza(string codigo_nave, [FromBody] EstadoNaveRequest request)
    {
        var estadosValidos = new[] { "patrulla", "hiperespacio", "combate" };
        if (string.IsNullOrWhiteSpace(request.Estado) || !estadosValidos.Contains(request.Estado.ToLower()))
        {
            return BadRequest(new { mensaje = "Estado inválido. Valores permitidos: 'patrulla', 'hiperespacio' o 'combate'." });
        }

        try
        {
            await _redisRepo.ActualizarEstadoNaveAsync(codigo_nave, request.Estado.ToLower());
            return Ok(new { mensaje = $"Estado de {codigo_nave} actualizado a: {request.Estado.ToLower()}. TTL renovado." });
        }
        catch (DatabaseOfflineException ex)
        {
            return StatusCode(503, new { mensaje = ex.Message });
        }
    }

    [HttpGet("rutas")]
    public async Task<IActionResult> CalcularRutaHiperespacial(
        [FromQuery][Required] string origen,
        [FromQuery][Required] string destino)
    {
        try
        {
            var resultado = await _redisRepo.ObtenerRutaRapidaAsync(origen, destino);
            return Ok(new { Origen = origen, Destino = destino, Ruta = resultado });
        }
        catch (DatabaseOfflineException ex)
        {
            return StatusCode(503, new { mensaje = ex.Message });
        }
    }
}