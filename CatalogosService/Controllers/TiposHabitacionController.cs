using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using Shared.DTOs;

namespace CatalogosService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TiposHabitacionController : ControllerBase
{
    private readonly TipoHabitacionRepository _repository;

    public TiposHabitacionController(TipoHabitacionRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TipoHabitacionDto>>> GetAll()
    {
        var tipos = await _repository.ObtenerTodosAsync();
        return Ok(tipos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<TipoHabitacionDto>> GetById(int id)
    {
        var tipo = await _repository.ObtenerPorIdAsync(id);
        if (tipo == null) return NotFound();
        return Ok(tipo);
    }

    [HttpPost]
    public async Task<ActionResult<TipoHabitacionDto>> Create([FromBody] TipoHabitacionDto dto)
    {
        var result = await _repository.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdTipoHabitacion }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TipoHabitacionDto>> Update(int id, [FromBody] TipoHabitacionDto dto)
    {
        var result = await _repository.ActualizarAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DesactivarAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }



}
