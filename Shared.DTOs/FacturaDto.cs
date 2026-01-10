namespace Shared.DTOs;

public class FacturaDto
{
    public int IdFactura { get; set; }
    public int IdReserva { get; set; }
    public int? IdUnicoUsuarioExterno { get; set; }
    public int? IdUnicoUsuario { get; set; }
    public decimal? SubtotalFactura { get; set; }
    public decimal? DescuentoTotalFactura { get; set; }
    public decimal? ImpuestoTotalFactura { get; set; }
    public DateTime? FechaCreacionFactura { get; set; }
    public string? EmailUsuarioExterno { get; set; }
    public string? EmailUsuario { get; set; }
    public bool? EstadoFactura { get; set; }
    public DateTime? FechaModificacionFactura { get; set; }
}
