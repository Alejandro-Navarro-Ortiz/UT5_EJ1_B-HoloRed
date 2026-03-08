using Microsoft.AspNetCore.Mvc;

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
        // Esta llamada es segura en multihilo gracias a nuestro lock en FlotaService
        bool exito = _flotaService.SolicitarAtraque(request.BahiaId, request.CodigoNave);

        if (exito)
        {
            return Ok(new { mensaje = $"Permiso concedido. Nave {request.CodigoNave} aterrizando en bahía {request.BahiaId}." });
        }

        // Código 409 Conflict si la bahía ya está ocupada (Condición de carrera evitada)
        return Conflict(new { mensaje = $"ALERTA DE COLISIÓN: La bahía {request.BahiaId} ya está ocupada." });
    }
}

public class AtraqueRequest { public string BahiaId { get; set; } public string CodigoNave { get; set; } }