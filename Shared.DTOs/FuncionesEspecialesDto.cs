namespace Shared.DTOs;

public class PreReservaResultDto
{
    public string IdHold { get; set; } = string.Empty;
    public object? Links { get; set; }
}

public class FacturaEmitidaDto
{
    public int IdFactura { get; set; }
    public int IdReserva { get; set; }
    public decimal? Subtotal { get; set; }
    public decimal? Descuento { get; set; }
    public decimal? Impuesto { get; set; }
    public decimal? Total { get; set; }
    public string? UrlPdf { get; set; }
    public object? Links { get; set; }
}

public class PreReservaRequest
{
    public string IdHabitacion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int NumeroHuespedes { get; set; }
    public int? DuracionHoldSeg { get; set; }
    public decimal? PrecioActual { get; set; }
    public int? IdUsuario { get; set; }
}

public class LoginRequest
{
    public string Correo { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
    public UsuarioInternoDto? Usuario { get; set; }
}
public class ReservaConfirmadaDto
{
    public int IdReserva { get; set; }
    public decimal? CostoTotalReserva { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string EstadoGeneral { get; set; }
    public bool? Estado { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Correo { get; set; }
    public string TipoDocumento { get; set; }
    public string Habitacion { get; set; }
    public decimal? PrecioNormal { get; set; }
    public decimal? PrecioActual { get; set; }
    public int? Capacidad { get; set; }
    public object _links { get; set; }
}

public class ConfirmarReservaInternaRequest
{
    public string IdHabitacion { get; set; } = string.Empty;
    public string IdHold { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    public string TipoDocumento { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int NumeroHuespedes { get; set; }
}