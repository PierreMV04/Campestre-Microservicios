namespace Shared.DTOs;

public class UsuarioInternoDto
{
    public int Id { get; set; }
    public int IdRol { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Correo { get; set; }
    public string? Clave { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? TipoDocumento { get; set; }
    public string? Documento { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public bool Estado { get; set; }
}
