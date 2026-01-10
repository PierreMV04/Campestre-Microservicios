namespace Shared.DTOs;

public class PagoDto
{
    public int IdPago { get; set; }
    public int IdMetodoPago { get; set; }
    public int? IdUnicoUsuarioExterno { get; set; }
    public int? IdUnicoUsuario { get; set; }
    public int? IdFactura { get; set; }
    public int? IdReserva { get; set; }
    public string? CuentaOrigenPago { get; set; }
    public string? CuentaDestinoPago { get; set; }
    public decimal? MontoTotalPago { get; set; }
    public bool? EstadoPago { get; set; }
    public DateTime? FechaModificacionPago { get; set; }

}
