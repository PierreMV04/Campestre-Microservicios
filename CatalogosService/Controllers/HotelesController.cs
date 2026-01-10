using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using Shared.DTOs;

namespace CatalogosService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HotelesController : ControllerBase
{
    private readonly HotelRepository _repository;

    public HotelesController(HotelRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetAll()
    {
        var hoteles = await _repository.ObtenerTodosAsync();
        return Ok(hoteles);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<HotelDto>> GetById(int id)
    {
        var hotel = await _repository.ObtenerPorIdAsync(id);
        if (hotel == null) return NotFound();
        return Ok(hotel);
    }

    [HttpPost]
    public async Task<ActionResult<HotelDto>> Create([FromBody] HotelDto dto)
    {
        var result = await _repository.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdHotel }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<HotelDto>> Update(int id, [FromBody] HotelDto dto)
    {
        var result = await _repository.ActualizarAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.EliminarAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
