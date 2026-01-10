using HotChocolate.Authorization;
using Shared.Data;
using Shared.DTOs;
using Shared.EventBus;

namespace HabitacionesService.GraphQL;

/// <summary>
/// Mutations GraphQL para Habitaciones - AUTENTICACIÓN OBLIGATORIA
/// </summary>
public class Mutation
{
    // ============ HABITACIONES ============
    [Authorize]
    public async Task<HabitacionDto> CreateHabitacion(
        [Service] HabitacionRepository repo,
        [Service] IEventBus eventBus,
        HabitacionInput input)
    {
        var dto = new HabitacionDto
        {
            IdHabitacion = input.IdHabitacion,
            IdHotel = input.IdHotel,
            IdTipoHabitacion = input.IdTipoHabitacion,
            IdCiudad = input.IdCiudad,
            NombreHabitacion = input.NombreHabitacion,
            CapacidadHabitacion = input.CapacidadHabitacion,
            PrecioNormalHabitacion = input.PrecioNormalHabitacion,
            PrecioActualHabitacion = input.PrecioActualHabitacion,
            EstadoHabitacion = input.EstadoHabitacion ?? true
        };

        var result = await repo.CrearAsync(dto);

        // Publicar evento
        await eventBus.PublishAsync(new HabitacionUpdatedEvent
        {
            IdHabitacion = result.IdHabitacion,
            IdHotel = result.IdHotel,
            Disponible = result.EstadoHabitacion
        });

        return result;
    }

    [Authorize]
    public async Task<HabitacionDto?> UpdateHabitacion(
        [Service] HabitacionRepository repo,
        [Service] IEventBus eventBus,
        string id,
        HabitacionInput input)
    {
        var dto = new HabitacionDto
        {
            IdHabitacion = id,
            IdHotel = input.IdHotel,
            IdTipoHabitacion = input.IdTipoHabitacion,
            IdCiudad = input.IdCiudad,
            NombreHabitacion = input.NombreHabitacion,
            CapacidadHabitacion = input.CapacidadHabitacion,
            PrecioNormalHabitacion = input.PrecioNormalHabitacion,
            PrecioActualHabitacion = input.PrecioActualHabitacion,
            EstadoHabitacion = input.EstadoHabitacion ?? true
        };

        var result = await repo.ActualizarAsync(id, dto);

        if (result != null)
        {
            await eventBus.PublishAsync(new HabitacionUpdatedEvent
            {
                IdHabitacion = result.IdHabitacion,
                IdHotel = result.IdHotel,
                Disponible = result.EstadoHabitacion
            });
        }

        return result;
    }

    [Authorize]
    public async Task<bool> DeleteHabitacion([Service] HabitacionRepository repo, string id)
        => await repo.EliminarAsync(id);

    // ============ IMÁGENES HABITACIÓN ============
    [Authorize]
    public async Task<ImagenHabitacionDto> CreateImagenHabitacion(
        [Service] ImagenHabitacionRepository repo,
        ImagenHabitacionInput input)
    {
        var dto = new ImagenHabitacionDto
        {
            IdImagenHabitacion = input.IdImagenHabitacion,
            IdHabitacion = input.IdHabitacion,
            UrlImagen = input.UrlImagen,
            EstadoImagen = input.EstadoImagen ?? true
        };

        return await repo.CrearAsync(dto);
    }


    [Authorize]
    public async Task<ImagenHabitacionDto?> UpdateImagenHabitacion(
        [Service] ImagenHabitacionRepository repo,
        int id,
        ImagenHabitacionInput input)
    {
        var dto = new ImagenHabitacionDto
        {
            IdImagenHabitacion = id,
            IdHabitacion = input.IdHabitacion,
            UrlImagen = input.UrlImagen,
            EstadoImagen = input.EstadoImagen ?? true
        };

        return await repo.ActualizarAsync(id, dto);
    }


    [Authorize]
    public async Task<bool> DeleteImagenHabitacion(
        [Service] ImagenHabitacionRepository repo,
        int id)
    {
        return await repo.EliminarAsync(id);
    }


    // ============ AMENIDADES POR HABITACIÓN ============
    [Authorize]
    public async Task<AmexHabDto> CreateAmexHab(
        [Service] AmexHabRepository repo,
        AmexHabInput input)
    {
        var dto = new AmexHabDto
        {
            IdHabitacion = input.IdHabitacion, // string ✔️
            IdAmenidad = input.IdAmenidad,
            EstadoAmexHab = input.EstadoAmexHab ?? true
        };

        return await repo.CrearAsync(dto);
    }

    [Authorize]
    public async Task<AmexHabDto?> UpdateAmexHab(
    [Service] AmexHabRepository repo,
    string idHabitacion,
    int idAmenidad,
    AmexHabInput input)
    {
        var dto = new AmexHabDto
        {
            IdHabitacion = idHabitacion,
            IdAmenidad = idAmenidad,
            EstadoAmexHab = input.EstadoAmexHab ?? true
        };

        return await repo.ActualizarAsync(idHabitacion, idAmenidad, dto);
    }





    [Authorize]
    public async Task<bool> DeleteAmexHab(
        [Service] AmexHabRepository repo,
        string idHabitacion,
        int idAmenidad)
    {
        return await repo.EliminarAsync(idHabitacion, idAmenidad);
    }

    // ============ DESCUENTOS ============
    [Authorize]
    public async Task<DescuentoDto> CreateDescuento(
        [Service] DescuentoRepository repo,
        DescuentoInput input)
    {
        var dto = new DescuentoDto
        {
            IdDescuento = input.IdDescuento,
            NombreDescuento = input.NombreDescuento,
            ValorDescuento = input.ValorDescuento,
            FechaInicioDescuento = input.FechaInicioDescuento,
            FechaFinDescuento = input.FechaFinDescuento,
            EstadoDescuento = input.EstadoDescuento ?? true
        };

        return await repo.CrearAsync(dto);
    }

    [Authorize]
    public async Task<DescuentoDto?> UpdateDescuento(
    [Service] DescuentoRepository repo,
    int id,
    DescuentoInput input)
    {
        var dto = new DescuentoDto
        {
            IdDescuento = id,
            NombreDescuento = input.NombreDescuento,
            ValorDescuento = input.ValorDescuento,
            FechaInicioDescuento = input.FechaInicioDescuento,
            FechaFinDescuento = input.FechaFinDescuento,
            EstadoDescuento = input.EstadoDescuento ?? true
        };

        return await repo.ActualizarAsync(id, dto);
    }


    [Authorize]
    public async Task<bool> DeleteDescuento(
        [Service] DescuentoRepository repo,
        int id)
    {
        return await repo.EliminarAsync(id);
    }

}

// ============ INPUT TYPES ============

public record HabitacionInput(
    string IdHabitacion,
    int IdHotel,
    int IdTipoHabitacion,
    int IdCiudad,
    string? NombreHabitacion,
    int? CapacidadHabitacion,
    decimal? PrecioNormalHabitacion,
    decimal? PrecioActualHabitacion,
    bool? EstadoHabitacion
);

public record ImagenHabitacionInput(
    int IdImagenHabitacion,
    string IdHabitacion,
    string? UrlImagen,
    bool? EstadoImagen
);


public record AmexHabInput(
    string IdHabitacion,
    int IdAmenidad,
    bool? EstadoAmexHab
);


public record DescuentoInput(
    int IdDescuento,
    string? NombreDescuento,
    decimal? ValorDescuento,
    DateTime? FechaInicioDescuento,
    DateTime? FechaFinDescuento,
    bool? EstadoDescuento
);

