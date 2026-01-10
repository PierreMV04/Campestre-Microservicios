using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using Shared.DTOs;

namespace CatalogosService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaisesController : ControllerBase
{
    private readonly PaisRepository _repository;

    public PaisesController(PaisRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PaisDto>>> GetAll()
    {
        var paises = await _repository.ObtenerTodosAsync();
        return Ok(paises);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PaisDto>> GetById(int id)
    {
        var pais = await _repository.ObtenerPorIdAsync(id);
        if (pais == null) return NotFound();
        return Ok(pais);
    }

    [HttpPost]
    public async Task<ActionResult<PaisDto>> Create([FromBody] PaisDto dto)
    {
        var result = await _repository.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdPais }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PaisDto>> Update(int id, [FromBody] PaisDto dto)
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
