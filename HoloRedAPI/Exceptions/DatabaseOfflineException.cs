namespace HoloRedAPI.Exceptions;

/// <summary>
/// Excepción semántica personalizada para la HoloRed.
/// Mejora la legibilidad y separa los errores de infraestructura (caída de BD) 
/// de los errores internos (bugs) de la API.
/// </summary>
public class DatabaseOfflineException : Exception
{
    // Constructor básico con mensaje
    public DatabaseOfflineException(string message) : base(message) { }

    // Constructor con mensaje y la excepción original que causó el error (InnerException)
    public DatabaseOfflineException(string message, Exception innerException) : base(message, innerException) { }
}