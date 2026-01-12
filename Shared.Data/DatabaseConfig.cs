namespace Shared.Data;

public static class DatabaseConfig
{
    // Cadena de conexión a la base de datos SQL Server en Somee.com
    // NOTA: En producción (Railway), esto será sobrescrito por variables de entorno
    public const string ConnectionString = "Server=db31651.public.databaseasp.net;Database=db31651;User Id=db31651;Password=prueba2020d;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";
}
