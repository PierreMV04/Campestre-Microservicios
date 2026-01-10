namespace Shared.DTOs;

public class AmexHabDto
{
    // char(10) NOT NULL
    public string IdHabitacion { get; set; } = null!;

    // int NOT NULL
    public int IdAmenidad { get; set; }

    // bit NULL
    public bool? EstadoAmexHab { get; set; }

    // datetime NULL
    public DateTime? FechaModificacionAmexHab { get; set; }
}
