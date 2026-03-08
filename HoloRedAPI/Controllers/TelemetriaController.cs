using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TelemetriaController : ControllerBase
{
    private readonly CassandraRepository _cassandraRepo;

    public TelemetriaController(CassandraRepository cassandraRepo)
    {
        _cassandraRepo = cassandraRepo;
    }

    [HttpPost("impacto")]
    public async Task<IActionResult> RegistrarImpacto([FromBody] ImpactoRequest r)
    {
        try
        {
            await _cassandraRepo.InsertarImpactoAsync(r.SectorId, r.Fecha, r.Atacante, r.Objetivo, r.Dano);
            return Ok(new { mensaje = "Impacto registrado en la matriz columnar." });
        }
        catch (Exception ex) when (ex.Message == "ERROR_RED_CASSANDRA")
        {
            return StatusCode(503, new { mensaje = "Sistema de logs dañado (Cassandra offline)." });
        }
    }

    [HttpGet("historial/{sector}")]
    public async Task<IActionResult> GetHistorial(string sector, [FromQuery] DateTime fecha)
    {
        try
        {
            var historial = await _cassandraRepo.ObtenerHistorialAsync(sector, fecha);
            return Ok(historial);
        }
        catch (Exception ex) when (ex.Message == "ERROR_RED_CASSANDRA")
        {
            return StatusCode(503, new { mensaje = "Sistema de logs dañado (Cassandra offline)." });
        }
    }
}

public class ImpactoRequest
{
    public string SectorId { get; set; }
    public DateTime Fecha { get; set; }
    public string Atacante { get; set; }
    public string Objetivo { get; set; }
    public int Dano { get; set; }
}