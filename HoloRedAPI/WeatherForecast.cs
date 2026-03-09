namespace HoloRedAPI
{
    /// <summary>
    /// Modelo de ejemplo de la plantilla de ASP.NET Core Web API.
    /// Retorna una simulación del pronóstico del clima.
    /// </summary>
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        // Propiedad calculada: Convierte la temperatura almacenada en Celsius a Fahrenheit
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}