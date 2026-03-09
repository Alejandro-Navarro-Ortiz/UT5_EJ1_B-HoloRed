using Microsoft.AspNetCore.Mvc;
using HoloRedAPI.Services;
using HoloRedAPI.Models;

namespace HoloRedAPI.Controllers;

/// <summary>
/// Controlador encargado de gestionar las operaciones de la flota espacial.
/// Proporciona endpoints para coordinar el aterrizaje y atraque de naves.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FlotaController : ControllerBase
{
    // Servicio que contiene la lógica de negocio de la flota (Singleton)
    private readonly FlotaService _flotaService;

    /// <summary>
    /// Constructor que inyecta el servicio de flota mediante Inyección de Dependencias.
    /// </summary>
    public FlotaController(FlotaService flotaService)
    {
        _flotaService = flotaService;
    }

    /// <summary>
    /// Endpoint POST para solicitar permiso de atraque en una bahía específica.
    /// </summary>
    /// <param name="request">Objeto con la ID de la bahía y el código de la nave.</param>
    /// <returns>HTTP 200 (Ok) si se concede el permiso, o HTTP 409 (Conflict) si está ocupada.</returns>
    [HttpPost("atraque")]
    public IActionResult SolicitarAtraque([FromBody] AtraqueRequest request)
    {
        // Delega la lógica de validación de disponibilidad al servicio
        bool exito = _flotaService.SolicitarAtraque(request.BahiaId, request.CodigoNave);

        if (exito)
        {
            return Ok(new { mensaje = $"Permiso concedido. Nave {request.CodigoNave} aterrizando en bahía {request.BahiaId}." });
        }

        return Conflict(new { mensaje = $"ALERTA DE COLISIÓN: La bahía {request.BahiaId} ya está ocupada." });
    }
}