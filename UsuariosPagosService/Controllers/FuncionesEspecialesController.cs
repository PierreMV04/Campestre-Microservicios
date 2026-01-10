using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using UsuariosPagosService.Repositories;

namespace UsuariosPagosService.Controllers;

[ApiController]
[Route("api/funciones-especiales")]
public class FuncionesEspecialesController : ControllerBase
{
    private readonly FuncionesEspecialesRepository _repository;

    public FuncionesEspecialesController(FuncionesEspecialesRepository repository)
    {
        _repository = repository;
    }

    // ============================================================
    // PRE-RESERVA (USUARIO INTERNO)
    // POST: api/funciones-especiales/prereserva
    // ============================================================
    [HttpPost("prereserva")]
    public ActionResult<PreReservaResultDto> CrearPreReserva(
        [FromBody] PreReservaRequest request)
    {
        if (request == null)
            return BadRequest("El cuerpo no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(request.IdHabitacion))
            return BadRequest("IdHabitacion es obligatorio.");

        if (request.FechaFin <= request.FechaInicio)
            return BadRequest("FechaFin debe ser mayor a FechaInicio.");

        if (request.NumeroHuespedes <= 0)
            return BadRequest("NumeroHuespedes debe ser mayor a 0.");

        try
        {
            var result = _repository.CrearPreReservaInterna(
                request.IdHabitacion,
                request.FechaInicio,
                request.FechaFin,
                request.NumeroHuespedes,
                request.DuracionHoldSeg,
                request.PrecioActual
            );

            return CreatedAtAction(
                nameof(ObtenerPreReserva),
                new { idHold = result.IdHold },
                result
            );
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    // ============================================================
    // GET PRE-RESERVA (placeholder / validación futura)
    // GET: api/funciones-especiales/prereserva/{idHold}
    // ============================================================
    [HttpGet("prereserva/{idHold}")]
    public ActionResult ObtenerPreReserva(string idHold)
    {
        if (string.IsNullOrWhiteSpace(idHold))
            return BadRequest("idHold es obligatorio.");

        // Actualmente no existe SP de consulta, solo confirmación/cancelación
        return Ok(new
        {
            IdHold = idHold,
            Estado = "ACTIVA"
        });
    }

    // ============================================================
    // CONFIRMAR RESERVA (USUARIO INTERNO)
    // POST: api/funciones-especiales/confirmar
    // ============================================================
    [HttpPost("confirmar")]
    public ActionResult<ReservaConfirmadaDto> ConfirmarReserva(
        [FromBody] ConfirmarReservaInternaRequest request)
    {
        if (request == null)
            return BadRequest("El cuerpo no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(request.IdHabitacion) ||
            string.IsNullOrWhiteSpace(request.IdHold) ||
            string.IsNullOrWhiteSpace(request.Correo))
            return BadRequest("IdHabitacion, IdHold y Correo son obligatorios.");

        try
        {
            var result = _repository.ConfirmarReservaUsuarioInterno(
                request.IdHabitacion,
                request.IdHold,
                request.Nombre,
                request.Apellido,
                request.Correo,
                request.TipoDocumento,
                request.Documento,
                request.FechaInicio,
                request.FechaFin,
                request.NumeroHuespedes
            );

            if (result == null)
                return NotFound("No se pudo confirmar la reserva.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    // ============================================================
    // EMITIR FACTURA (USUARIO INTERNO)
    // POST: api/funciones-especiales/emitir-factura
    // ============================================================
    [HttpPost("emitir-factura")]
    public ActionResult<FacturaEmitidaDto> EmitirFactura(
        [FromQuery] int idReserva,
        [FromQuery] string? correo,
        [FromQuery] string? nombre,
        [FromQuery] string? apellido,
        [FromQuery] string? tipoDocumento,
        [FromQuery] string? documento)
    {
        if (idReserva <= 0)
            return BadRequest("idReserva es obligatorio.");

        try
        {
            var factura = _repository.EmitirInterno(
                idReserva,
                correo,
                nombre,
                apellido,
                tipoDocumento,
                documento
            );

            if (factura == null)
                return NotFound("No se pudo emitir la factura.");

            return Ok(factura);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    // ============================================================
    // CANCELAR PRE-RESERVA
    // DELETE: api/funciones-especiales/prereserva/{idHold}
    // ============================================================
    [HttpDelete("prereserva/{idHold}")]
    public IActionResult CancelarPreReserva(string idHold)
    {
        if (string.IsNullOrWhiteSpace(idHold))
            return BadRequest("idHold es obligatorio.");

        try
        {
            _repository.CancelarPreReserva(idHold);
            return Ok(new { mensaje = "Pre-reserva cancelada correctamente." });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    // ============================================================
    // EXPIRAR PRE-RESERVA (USO INTERNO / JOB)
    // POST: api/funciones-especiales/expirar/{idHold}
    // ============================================================
    [HttpPost("expirar/{idHold}")]
    public IActionResult ExpirarPreReserva(string idHold)
    {
        if (string.IsNullOrWhiteSpace(idHold))
            return BadRequest("idHold es obligatorio.");

        try
        {
            var idReserva = _repository.ObtenerIdReservaPorHold(idHold);
            _repository.ExpirarReserva(idReserva);

            return Ok(new
            {
                mensaje = "Pre-reserva expirada correctamente.",
                idReserva
            });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
