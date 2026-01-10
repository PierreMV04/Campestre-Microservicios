using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers.Integracion;

[ApiController]
[Route("api/integracion/usuarios/externos")]
public class IntegracionUsuarioExternoController : ControllerBase
{
    private readonly HttpClient _http;

    public IntegracionUsuarioExternoController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("RecaApi");
    }

    /// <summary>
    /// Crea un nuevo usuario externo a partir del canal de reservas.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] object body)
    {
        if (body == null)
            return BadRequest("El cuerpo no puede estar vacío.");

        var response = await _http.PostAsJsonAsync(
            "/api/v1/hoteles/usuarios/externo",
            body
        );

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, content);

        return Ok(content);
    }
}
