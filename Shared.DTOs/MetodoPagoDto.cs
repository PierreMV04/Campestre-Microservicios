namespace Shared.DTOs;

public class MetodoPagoDto
{
    public int IdMetodoPago { get; set; }
    public string? NombreMetodoPago { get; set; }
    public bool? EstadoMetodoPago { get; set; }
    public DateTime? FechaModificacionMetodoPago { get; set; }
}
