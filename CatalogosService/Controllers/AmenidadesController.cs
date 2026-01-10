using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using Shared.DTOs;

namespace CatalogosService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AmenidadesController : ControllerBase
{
    private readonly AmenidadRepository _repository;

    public AmenidadesController(AmenidadRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AmenidadDto>>> GetAll()
    {
        var amenidades = await _repository.ObtenerTodosAsync();
        return Ok(amenidades);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<AmenidadDto>> GetById(int id)
    {
        var amenidad = await _repository.ObtenerPorIdAsync(id);
        if (amenidad == null) return NotFound();
        return Ok(amenidad);
    }

    [HttpPost]
    public async Task<ActionResult<AmenidadDto>> Create([FromBody] AmenidadDto dto)
    {
        var result = await _repository.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdAmenidad }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AmenidadDto>> Update(int id, [FromBody] AmenidadDto dto)
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
