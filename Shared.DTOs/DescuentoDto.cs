namespace Shared.DTOs;

public class DescuentoDto
{
    public int IdDescuento { get; set; }
    public string? NombreDescuento { get; set; }
    public decimal? ValorDescuento { get; set; }
    public DateTime? FechaInicioDescuento { get; set; }
    public DateTime? FechaFinDescuento { get; set; }
    public bool? EstadoDescuento { get; set; }
    public DateTime? FechaModificacionDescuento { get; set; }
}
