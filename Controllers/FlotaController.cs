using Microsoft.AspNetCore.Mvc;
using HoloRedAPI.Services;
using HoloRedAPI.Models;

namespace HoloRedAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlotaController : ControllerBase
{
    private readonly FlotaService _flotaService;

    public FlotaController(FlotaService flotaService)
    {
        _flotaService = flotaService;
    }

    [HttpPost("atraque")]
    public IActionResult SolicitarAtraque([FromBody] AtraqueRequest request)
    {
        bool exito = _flotaService.SolicitarAtraque(request.BahiaId, request.CodigoNave);

        if (exito)
        {
            return Ok(new { mensaje = $"Permiso concedido. Nave {request.CodigoNave} aterrizando en bahía {request.BahiaId}." });
        }

        return Conflict(new { mensaje = $"ALERTA DE COLISIÓN: La bahía {request.BahiaId} ya está ocupada." });
    }
}