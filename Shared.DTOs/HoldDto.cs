namespace Shared.DTOs;

public class HoldDto
{
    public string IdHold { get; set; } = null!;
    public string IdHabitacion { get; set; } = null!;
    public int IdReserva { get; set; }
    public int? TiempoHold { get; set; }
    public DateTime? FechaInicioHold { get; set; }
    public DateTime? FechaFinalHold { get; set; }
    public bool? EstadoHold { get; set; }
}
