namespace Shared.DTOs;

public class HabxResDto
{
    // PK
    public int IdHabxRes { get; set; }

    // char(10) NOT NULL
    public string IdHabitacion { get; set; } = null!;

    // FK NOT NULL
    public int IdReserva { get; set; }

    // int NULL
    public int? CapacidadReservaHabxRes { get; set; }

    // decimal(8,2) NULL
    public decimal? CostoCalculadoHabxRes { get; set; }

    // decimal(8,2) NULL
    public decimal? DescuentoHabxRes { get; set; }

    // decimal(8,2) NULL
    public decimal? ImpuestosHabxRes { get; set; }

    // bit NULL
    public bool? EstadoHabxRes { get; set; }

    // datetime NULL
    public DateTime? FechaModificacionHabxRes { get; set; }
}
