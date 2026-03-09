namespace HoloRedAPI.Exceptions;

// Excepción semántica personalizada. 
// Mejora la legibilidad y separa los errores de red de los errores internos de la API.
public class DatabaseOfflineException : Exception
{
    public DatabaseOfflineException(string message) : base(message) { }
    public DatabaseOfflineException(string message, Exception innerException) : base(message, innerException) { }
}