using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers.Integracion;

[ApiController]
[Route("api/integracion/reservas")]
public class IntegracionReservasController : ControllerBase
{
    private readonly HttpClient _http;

    public IntegracionReservasController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("RecaApi");
    }

    /// <summary>
    /// Busca una reserva por su identificador interno.
    /// </summary>
    /// <param name="idReserva">Identificador interno de la reserva</param>
    [HttpGet]
    public async Task<IActionResult> Buscar([FromQuery] int? idReserva)
    {
        if (!idReserva.HasValue || idReserva <= 0)
            return BadRequest("idReserva es obligatorio y debe ser mayor que 0.");

        var response = await _http.GetAsync(
            $"/api/v1/hoteles/reserva?idReserva={idReserva}"
        );

        var content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return NotFound();

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, content);

        return Ok(content);
    }
}
