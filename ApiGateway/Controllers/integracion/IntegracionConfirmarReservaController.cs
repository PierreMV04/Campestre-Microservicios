using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers.Integracion;

[ApiController]
[Route("api/integracion/reservas")]
public class IntegracionConfirmarReservaController : ControllerBase
{
    private readonly HttpClient _http;

    public IntegracionConfirmarReservaController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("RecaApi");
    }

    /// <summary>
    /// Confirma una reserva definitiva a partir de un hold (pre–reserva).
    /// </summary>
    [HttpPost("confirmar")]
    public async Task<IActionResult> ConfirmarReserva(
        [FromBody] ConfirmarReservaRequest req)
    {
        if (req == null)
            return BadRequest("El cuerpo no puede estar vacío.");

        // 👉 Forward exacto hacia RECA
        var response = await _http.PostAsJsonAsync(
            "/api/v1/hoteles/book",
            req
        );

        // Si RECA devuelve error, lo propagamos
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, error);
        }

        var content = await response.Content.ReadAsStringAsync();

        // 🔔 Aquí PODRÍAS publicar evento RabbitMQ
        // ReservaConfirmadaEvent

        // RECA devuelve 201 Created
        return StatusCode((int)response.StatusCode, content);
    }
}
public class ConfirmarReservaRequest
{
    public string idHabitacion { get; set; }
    public string idHold { get; set; }
    public string nombre { get; set; }
    public string apellido { get; set; }
    public string correo { get; set; }
    public string tipoDocumento { get; set; }
    public string documento { get; set; }
    public DateTime fechaInicio { get; set; }
    public DateTime fechaFin { get; set; }
    public int numeroHuespedes { get; set; }
}
