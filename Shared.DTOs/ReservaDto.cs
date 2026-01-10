namespace Shared.DTOs;

public class ReservaDto
{
    public int IdReserva { get; set; }
    public int? IdUnicoUsuario { get; set; }
    public int? IdUnicoUsuarioExterno { get; set; }
    public decimal? CostoTotalReserva { get; set; }
    public DateTime? FechaRegistroReserva { get; set; }
    public DateTime? FechaInicioReserva { get; set; }
    public DateTime? FechaFinalReserva { get; set; }
    public string? EstadoGeneralReserva { get; set; }
    public bool? EstadoReserva { get; set; }
    public DateTime? FechaModificacionReserva { get; set; }
}
