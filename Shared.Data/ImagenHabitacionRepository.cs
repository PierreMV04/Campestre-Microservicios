using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class ImagenHabitacionRepository
{
    private readonly string _connectionString;

    public ImagenHabitacionRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<ImagenHabitacionDto>> ObtenerTodosAsync()
    {
        var lista = new List<ImagenHabitacionDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_IMAGEN_HABITACION, ID_HABITACION, URL_IMAGEN,
                   ESTADO_IMAGEN, FECHA_MODIFICACION_IMAGEN_HABITACION
            FROM IMAGEN_HABITACION", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<ImagenHabitacionDto?> ObtenerPorIdAsync(int idImagenHabitacion)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_IMAGEN_HABITACION, ID_HABITACION, URL_IMAGEN,
                   ESTADO_IMAGEN, FECHA_MODIFICACION_IMAGEN_HABITACION
            FROM IMAGEN_HABITACION
            WHERE ID_IMAGEN_HABITACION = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idImagenHabitacion;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<List<ImagenHabitacionDto>> ObtenerPorHabitacionAsync(string idHabitacion)
    {
        var lista = new List<ImagenHabitacionDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_IMAGEN_HABITACION, ID_HABITACION, URL_IMAGEN,
                   ESTADO_IMAGEN, FECHA_MODIFICACION_IMAGEN_HABITACION
            FROM IMAGEN_HABITACION
            WHERE ID_HABITACION = @ID_HAB", cn);

        cmd.Parameters.Add("@ID_HAB", SqlDbType.Char, 10).Value = idHabitacion;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<ImagenHabitacionDto> CrearAsync(ImagenHabitacionDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO IMAGEN_HABITACION
            (ID_IMAGEN_HABITACION, ID_HABITACION, URL_IMAGEN,
             ESTADO_IMAGEN, FECHA_MODIFICACION_IMAGEN_HABITACION)
            VALUES (@ID, @HAB, @URL, @ESTADO, @FECHA)", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = dto.IdImagenHabitacion;
        cmd.Parameters.Add("@HAB", SqlDbType.Char, 10).Value = dto.IdHabitacion;
        cmd.Parameters.Add("@URL", SqlDbType.VarChar, 255).Value =
            (object?)dto.UrlImagen ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoImagen ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        dto.FechaModificacionImagenHabitacion = now;
        return dto;
    }

    public async Task<ImagenHabitacionDto?> ActualizarAsync(int idImagenHabitacion, ImagenHabitacionDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE IMAGEN_HABITACION SET
                ID_HABITACION = @HAB,
                URL_IMAGEN = @URL,
                ESTADO_IMAGEN = @ESTADO,
                FECHA_MODIFICACION_IMAGEN_HABITACION = @FECHA
            WHERE ID_IMAGEN_HABITACION = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idImagenHabitacion;
        cmd.Parameters.Add("@HAB", SqlDbType.Char, 10).Value = dto.IdHabitacion;
        cmd.Parameters.Add("@URL", SqlDbType.VarChar, 255).Value =
            (object?)dto.UrlImagen ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoImagen ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(int idImagenHabitacion)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"
UPDATE IMAGEN_HABITACION
SET ESTADO_IMAGEN = 0
WHERE ID_IMAGEN_HABITACION = @ID
", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idImagenHabitacion;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static ImagenHabitacionDto Map(SqlDataReader dr) => new()
    {
        IdImagenHabitacion = dr.GetInt32(0),
        IdHabitacion = dr.GetString(1).Trim(),
        UrlImagen = dr.IsDBNull(2) ? null : dr.GetString(2),
        EstadoImagen = dr.IsDBNull(3) ? null : dr.GetBoolean(3),
        FechaModificacionImagenHabitacion = dr.IsDBNull(4) ? null : dr.GetDateTime(4)
    };
}
