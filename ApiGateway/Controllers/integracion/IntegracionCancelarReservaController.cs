using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers.Integracion;

[ApiController]
[Route("api/integracion/reservas")]
public class IntegracionCancelarReservaController : ControllerBase
{
    private readonly HttpClient _http;

    public IntegracionCancelarReservaController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("RecaApi");
    }

    [HttpDelete("cancelar")]
    public async Task<IActionResult> CancelarReserva(
        [FromQuery] int idReserva)
    {
        // Construir URL hacia RECA
        var response = await _http.DeleteAsync(
            $"/api/v1/hoteles/cancel?idReserva={idReserva}"
        );

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode,
                await response.Content.ReadAsStringAsync());
        }

        // 🔔 Opcional: publicar evento RabbitMQ
        // ReservaCanceladaEvent

        return Ok(); // RECA devuelve {}
    }
}
