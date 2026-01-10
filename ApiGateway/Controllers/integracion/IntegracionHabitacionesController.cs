using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace ApiGateway.Controllers.Integracion;

[ApiController]
[Route("api/integracion/habitaciones")]
public class IntegracionHabitacionesController : ControllerBase
{
    private readonly HttpClient _http;

    public IntegracionHabitacionesController(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("RecaApi");
    }

    /// <summary>
    /// Busca habitaciones disponibles según rango de fechas, tipo, capacidad y precios.
    /// Los parámetros son opcionales; si no se envían no se aplican filtros.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Buscar(
        [FromQuery] DateTime? date_from,
        [FromQuery] DateTime? date_to,
        [FromQuery] string? tipo_habitacion,
        [FromQuery] int? capacidad,
        [FromQuery] decimal? precio_min,
        [FromQuery] decimal? precio_max)
    {
        // Construcción dinámica del querystring
        var query = new Dictionary<string, string?>();

        if (date_from.HasValue)
            query["date_from"] = date_from.Value.ToString("yyyy-MM-ddTHH:mm:ss");

        if (date_to.HasValue)
            query["date_to"] = date_to.Value.ToString("yyyy-MM-ddTHH:mm:ss");

        if (!string.IsNullOrWhiteSpace(tipo_habitacion))
            query["tipo_habitacion"] = tipo_habitacion;

        if (capacidad.HasValue)
            query["capacidad"] = capacidad.Value.ToString();

        if (precio_min.HasValue)
            query["precio_min"] = precio_min.Value.ToString();

        if (precio_max.HasValue)
            query["precio_max"] = precio_max.Value.ToString();

        var url = QueryHelpers.AddQueryString(
            "/api/v1/hoteles/search",
            query
        );

        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, error);
        }

        var content = await response.Content.ReadAsStringAsync();
        return Ok(content);
    }
}
