namespace Shared.DTOs;

public class PdfDto
{
    public int IdPdf { get; set; }
    public int? IdFactura { get; set; }
    public string? UrlPdf { get; set; }
    public bool? EstadoPdf { get; set; }
    public DateTime? FechaModificacionPdf { get; set; }
}
