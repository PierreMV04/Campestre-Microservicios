using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using Shared.DTOs;

namespace CatalogosService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MetodosPagoController : ControllerBase
{
    private readonly MetodoPagoRepository _repository;

    public MetodosPagoController(MetodoPagoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MetodoPagoDto>>> GetAll()
    {
        var metodos = await _repository.ObtenerTodosAsync();
        return Ok(metodos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<MetodoPagoDto>> GetById(int id)
    {
        var metodo = await _repository.ObtenerPorIdAsync(id);
        if (metodo == null) return NotFound();
        return Ok(metodo);
    }

    [HttpPost]
    public async Task<ActionResult<MetodoPagoDto>> Create([FromBody] MetodoPagoDto dto)
    {
        var result = await _repository.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdMetodoPago }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MetodoPagoDto>> Update(int id, [FromBody] MetodoPagoDto dto)
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
