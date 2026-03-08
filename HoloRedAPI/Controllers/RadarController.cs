using Microsoft.AspNetCore.Mvc;

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
        try
        {
            await _redisRepo.ActualizarEstadoNaveAsync(codigo_nave, request.Estado);
            return Ok(new { mensaje = $"Estado de {codigo_nave} actualizado a: {request.Estado}. TTL renovado." });
        }
        catch (Exception ex) when (ex.Message == "ERROR_RED_REDIS")
        {
            return StatusCode(503, new { mensaje = "Fallo en el Radar (Redis desconectado)." });
        }
    }
}

// DTO para recibir el estado en formato JSON
public class EstadoNaveRequest { public string Estado { get; set; } }