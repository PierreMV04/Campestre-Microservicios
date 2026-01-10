using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using Shared.DTOs;

namespace CatalogosService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CiudadesController : ControllerBase
{
    private readonly CiudadRepository _repository;

    public CiudadesController(CiudadRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CiudadDto>>> GetAll()
    {
        var ciudades = await _repository.ObtenerTodosAsync();
        return Ok(ciudades);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CiudadDto>> GetById(int id)
    {
        var ciudad = await _repository.ObtenerPorIdAsync(id);
        if (ciudad == null) return NotFound();
        return Ok(ciudad);
    }

    [HttpPost]
    public async Task<ActionResult<CiudadDto>> Create([FromBody] CiudadDto dto)
    {
        var result = await _repository.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdCiudad }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CiudadDto>> Update(int id, [FromBody] CiudadDto dto)
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
