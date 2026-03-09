using Microsoft.AspNetCore.Mvc;
using HoloRedAPI.Repositories;
using HoloRedAPI.Models;
using HoloRedAPI.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace HoloRedAPI.Controllers;

/// <summary>
/// Controlador para el sistema de radar y posicionamiento.
/// Utiliza Redis para lecturas y escrituras ultrarrápidas y almacenamiento en caché.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RadarController : ControllerBase
{
    private readonly RedisRepository _redisRepo;

    public RadarController(RedisRepository redisRepo)
    {
        _redisRepo = redisRepo;
    }

    /// <summary>
    /// Endpoint POST para actualizar temporalmente el estado de una baliza o nave.
    /// </summary>
    /// <param name="codigo_nave">Código identificador de la nave.</param>
    /// <param name="request">Estado a actualizar (patrulla, hiperespacio o combate).</param>
    [HttpPost("baliza/{codigo_nave}")]
    public async Task<IActionResult> ActualizarBaliza(string codigo_nave, [FromBody] EstadoNaveRequest request)
    {
        // Validación de los estados permitidos
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

    /// <summary>
    /// Endpoint GET para calcular o recuperar de la caché una ruta hiperespacial.
    /// </summary>
    [HttpGet("rutas")]
    public async Task<IActionResult> CalcularRutaHiperespacial(
        [FromQuery][Required] string origen,
        [FromQuery][Required] string destino)
    {
        try
        {
            // Retorna la ruta rápidamente desde Redis si existe, si no, simula el cálculo
            var resultado = await _redisRepo.ObtenerRutaRapidaAsync(origen, destino);
            return Ok(new { Origen = origen, Destino = destino, Ruta = resultado });
        }
        catch (DatabaseOfflineException ex)
        {
            return StatusCode(503, new { mensaje = ex.Message });
        }
    }
}