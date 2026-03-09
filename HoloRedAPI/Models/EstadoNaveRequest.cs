namespace HoloRedAPI.Models;

// Estos modelos representan la estructura de los datos (DTOs) 
// que la API espera recibir en el cuerpo (Body) de las peticiones HTTP.

/// <summary>
/// Modelo para la petición de actualización del estado del radar de una nave.
/// </summary>
public class EstadoNaveRequest
{
    public string Estado { get; set; } = string.Empty;
}

/// <summary>
/// Modelo para solicitar el atraque en una bahía de la flota.
/// </summary>
public class AtraqueRequest
{
    public string BahiaId { get; set; } = string.Empty;
    public string CodigoNave { get; set; } = string.Empty;
}

/// <summary>
/// Modelo que representa un registro de impacto o daño en telemetría.
/// </summary>
public class ImpactoRequest
{
    public string SectorId { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Atacante { get; set; } = string.Empty;
    public string Objetivo { get; set; } = string.Empty;
    public int Dano { get; set; }
}