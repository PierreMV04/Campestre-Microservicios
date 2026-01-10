using Microsoft.AspNetCore.Mvc;
using Shared.Data;
using Shared.DTOs;

namespace UsuariosPagosService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfsController : ControllerBase
{
    private readonly PdfRepository _repository;

    public PdfsController(PdfRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PdfDto>>> GetAll()
    {
        var pdfs = await _repository.ObtenerTodosAsync();
        return Ok(pdfs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PdfDto>> GetById(int id)
    {
        var pdf = await _repository.ObtenerPorIdAsync(id);
        if (pdf == null) return NotFound();
        return Ok(pdf);
    }

    [HttpGet("factura/{idFactura}")]
    public async Task<ActionResult<PdfDto>> GetByFactura(int idFactura)
    {
        var pdf = await _repository.ObtenerPorFacturaAsync(idFactura);
        if (pdf == null) return NotFound();
        return Ok(pdf);
    }

    [HttpPost]
    public async Task<ActionResult<PdfDto>> Create([FromBody] PdfDto dto)
    {
        var result = await _repository.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.IdPdf }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PdfDto>> Update(int id, [FromBody] PdfDto dto)
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
