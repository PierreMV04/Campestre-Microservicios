namespace Shared.DTOs;

public class CiudadDto
{
    public int IdCiudad { get; set; }
    public int IdPais { get; set; }
    public string? NombreCiudad { get; set; }
    public bool? EstadoCiudad { get; set; }
    public DateTime? FechaModificacionCiudad { get; set; }
}
