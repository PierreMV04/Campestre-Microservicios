using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using Shared.DTOs;

namespace CatalogosService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly RolRepository _repository;

    public RolesController(RolRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RolDto>>> GetAll()
    {
        var roles = await _repository.ObtenerTodosAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<RolDto>> GetById(int id)
    {
        var rol = await _repository.ObtenerPorIdAsync(id);
        if (rol == null) return NotFound();
        return Ok(rol);
    }

    [HttpPost]
    public async Task<ActionResult<RolDto>> Create([FromBody] RolDto dto)
    {
        var result = await _repository.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdRol }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RolDto>> Update(int id, [FromBody] RolDto dto)
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
