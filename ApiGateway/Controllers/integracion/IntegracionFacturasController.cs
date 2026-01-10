using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers.Integracion;

[ApiController]
[Route("api/integracion/facturas")]
public class IntegracionFacturasController : ControllerBase
{
    private readonly HttpClient _http;

    public IntegracionFacturasController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("RecaApi");
    }

    /// <summary>
    /// Emite la factura asociada a una reserva específica.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> EmitirFactura(
        [FromBody] EmitirFacturaRequest req)
    {
        if (req == null)
            return BadRequest("El cuerpo no puede estar vacío.");

        // 👉 Forward exacto hacia RECA
        var response = await _http.PostAsJsonAsync(
            "/api/v1/hoteles/invoices",
            req
        );

        // Propagar errores tal cual
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, error);
        }

        var content = await response.Content.ReadAsStringAsync();

        // 🔔 Aquí podrías publicar evento RabbitMQ
        // FacturaEmitidaEvent

        return Ok(content);
    }
}
public class EmitirFacturaRequest
{
    public int idReserva { get; set; }
    public string nombre { get; set; }
    public string apellido { get; set; }
    public string tipoDocumento { get; set; }
    public string documento { get; set; }
    public string correo { get; set; }
}
