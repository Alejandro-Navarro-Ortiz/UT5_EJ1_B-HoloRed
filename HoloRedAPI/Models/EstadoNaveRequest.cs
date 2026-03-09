namespace HoloRedAPI.Models;

public class EstadoNaveRequest
{
    public string Estado { get; set; } = string.Empty;
}

public class AtraqueRequest
{
    public string BahiaId { get; set; } = string.Empty;
    public string CodigoNave { get; set; } = string.Empty;
}

public class ImpactoRequest
{
    public string SectorId { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Atacante { get; set; } = string.Empty;
    public string Objetivo { get; set; } = string.Empty;
    public int Dano { get; set; }
}