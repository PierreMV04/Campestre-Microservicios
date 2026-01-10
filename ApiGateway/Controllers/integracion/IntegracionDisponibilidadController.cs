using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers.Integracion;

[ApiController]
[Route("api/integracion/disponibilidad")]
public class IntegracionDisponibilidadController : ControllerBase
{
    private readonly HttpClient _http;

    public IntegracionDisponibilidadController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("RecaApi");
    }

    /// <summary>
    /// Valida la disponibilidad de una habitación en un rango de fechas.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ValidarDisponibilidad(
        [FromBody] ValidarDisponibilidadRequest req)
    {
        if (req == null)
            return BadRequest("El cuerpo no puede estar vacío.");

        // 👉 Forward exacto hacia RECA
        var response = await _http.PostAsJsonAsync(
            "/api/v1/hoteles/availability",
            req
        );

        // Propagar errores tal cual
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            // 404, 400, 500, etc.
            return StatusCode((int)response.StatusCode, error);
        }

        var content = await response.Content.ReadAsStringAsync();

        return Ok(content);
    }
}
public class ValidarDisponibilidadRequest
{
    public string idHabitacion { get; set; }
    public DateTime fechaInicio { get; set; }
    public DateTime fechaFin { get; set; }
}
