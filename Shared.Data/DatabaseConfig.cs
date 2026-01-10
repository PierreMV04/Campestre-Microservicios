namespace Shared.Data;

public static class DatabaseConfig
{
    // Cadena de conexión a la base de datos SQL Server en Somee.com
    // NOTA: En producción (Railway), esto será sobrescrito por variables de entorno
    public const string ConnectionString = "Server=db31616.public.databaseasp.net;Database=db31616;User Id=db31616;Password=eG_3B%4y8d!K;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";
}
