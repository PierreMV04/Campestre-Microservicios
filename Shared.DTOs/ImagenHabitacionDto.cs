namespace Shared.DTOs;

public class ImagenHabitacionDto
{
    public int IdImagenHabitacion { get; set; }
    public string IdHabitacion { get; set; } = null!;
    public string? UrlImagen { get; set; }
    public bool? EstadoImagen { get; set; }
    public DateTime? FechaModificacionImagenHabitacion { get; set; }
}
