namespace Shared.DTOs;
public class HabitacionDto
{
    public string IdHabitacion { get; set; } = string.Empty;
    public int IdTipoHabitacion { get; set; }
    public int IdCiudad { get; set; }
    public int IdHotel { get; set; }
    public string? NombreHabitacion { get; set; }
    public decimal? PrecioNormalHabitacion { get; set; }
    public decimal? PrecioActualHabitacion { get; set; }
    public int? CapacidadHabitacion { get; set; }
    public bool? EstadoHabitacion { get; set; }
    public DateTime? FechaRegistroHabitacion { get; set; }
    public bool? EstadoActivoHabitacion { get; set; }
    public DateTime? FechaModificacionHabitacion { get; set; }
}
