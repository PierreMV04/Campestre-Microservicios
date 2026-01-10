using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers.Integracion;

[ApiController]
[Route("api/integracion/prereserva")]
public class IntegracionPreReservaController : ControllerBase
{
    private readonly HttpClient _http;

    public IntegracionPreReservaController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("RecaApi");
    }

    /// <summary>
    /// Crea una pre–reserva (hold) sobre una habitación para un rango de fechas
    /// y número de huéspedes.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] object request)
    {
        if (request == null)
            return BadRequest("El cuerpo no puede estar vacío.");

        var response = await _http.PostAsJsonAsync(
            "/api/v1/hoteles/hold",
            request
        );

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, content);

        return Ok(content);
    }
}
