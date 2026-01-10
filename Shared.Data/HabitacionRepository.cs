using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class HabitacionRepository
{
    private readonly string _connectionString;

    public HabitacionRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<HabitacionDto>> ObtenerTodasAsync()
    {
        var lista = new List<HabitacionDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HABITACION, ID_TIPO_HABITACION, ID_CIUDAD, ID_HOTEL, 
                   NOMBRE_HABITACION, PRECIO_NORMAL_HABITACION, PRECIO_ACTUAL_HABITACION,
                   CAPACIDAD_HABITACION, ESTADO_HABITACION, FECHA_REGISTRO_HABITACION,
                   ESTADO_ACTIVO_HABITACION, FECHA_MODIFICACION_HABITACION
            FROM HABITACION", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(MapHabitacion(dr));

        return lista;
    }

    public async Task<HabitacionDto?> ObtenerPorIdAsync(string idHabitacion)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HABITACION, ID_TIPO_HABITACION, ID_CIUDAD, ID_HOTEL, 
                   NOMBRE_HABITACION, PRECIO_NORMAL_HABITACION, PRECIO_ACTUAL_HABITACION,
                   CAPACIDAD_HABITACION, ESTADO_HABITACION, FECHA_REGISTRO_HABITACION,
                   ESTADO_ACTIVO_HABITACION, FECHA_MODIFICACION_HABITACION
            FROM HABITACION
            WHERE ID_HABITACION = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = idHabitacion;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        if (await dr.ReadAsync())
            return MapHabitacion(dr);

        return null;
    }

    public async Task<List<HabitacionDto>> ObtenerPorHotelAsync(int idHotel)
    {
        var lista = new List<HabitacionDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HABITACION, ID_TIPO_HABITACION, ID_CIUDAD, ID_HOTEL, 
                   NOMBRE_HABITACION, PRECIO_NORMAL_HABITACION, PRECIO_ACTUAL_HABITACION,
                   CAPACIDAD_HABITACION, ESTADO_HABITACION, FECHA_REGISTRO_HABITACION,
                   ESTADO_ACTIVO_HABITACION, FECHA_MODIFICACION_HABITACION
            FROM HABITACION
            WHERE ID_HOTEL = @ID_HOTEL", cn);

        cmd.Parameters.Add("@ID_HOTEL", SqlDbType.Int).Value = idHotel;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(MapHabitacion(dr));

        return lista;
    }

    public async Task<HabitacionDto> CrearAsync(HabitacionDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO HABITACION
            (ID_HABITACION, ID_TIPO_HABITACION, ID_CIUDAD, ID_HOTEL, NOMBRE_HABITACION,
             PRECIO_NORMAL_HABITACION, PRECIO_ACTUAL_HABITACION, CAPACIDAD_HABITACION,
             ESTADO_HABITACION, FECHA_REGISTRO_HABITACION, ESTADO_ACTIVO_HABITACION, FECHA_MODIFICACION_HABITACION)
            VALUES (@ID, @TIPO, @CIUDAD, @HOTEL, @NOMBRE, @PRECIO_NORMAL, @PRECIO_ACTUAL, 
                    @CAPACIDAD, @ESTADO, @FECHA_REG, @ESTADO_ACTIVO, @FECHA_MOD)", cn);

        cmd.Parameters.AddWithValue("@ID", dto.IdHabitacion);
        cmd.Parameters.AddWithValue("@TIPO", dto.IdTipoHabitacion);
        cmd.Parameters.AddWithValue("@CIUDAD", dto.IdCiudad);
        cmd.Parameters.AddWithValue("@HOTEL", dto.IdHotel);
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreHabitacion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PRECIO_NORMAL", (object?)dto.PrecioNormalHabitacion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PRECIO_ACTUAL", (object?)dto.PrecioActualHabitacion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CAPACIDAD", (object?)dto.CapacidadHabitacion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoHabitacion ?? true);
        cmd.Parameters.AddWithValue("@FECHA_REG", now);
        cmd.Parameters.AddWithValue("@ESTADO_ACTIVO", dto.EstadoActivoHabitacion ?? true);
        cmd.Parameters.AddWithValue("@FECHA_MOD", now);

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        dto.FechaRegistroHabitacion = now;
        dto.FechaModificacionHabitacion = now;
        return dto;
    }

    public async Task<HabitacionDto?> ActualizarAsync(string idHabitacion, HabitacionDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE HABITACION SET
                ID_TIPO_HABITACION = @TIPO,
                ID_CIUDAD = @CIUDAD,
                ID_HOTEL = @HOTEL,
                NOMBRE_HABITACION = @NOMBRE,
                PRECIO_NORMAL_HABITACION = @PRECIO_NORMAL,
                PRECIO_ACTUAL_HABITACION = @PRECIO_ACTUAL,
                CAPACIDAD_HABITACION = @CAPACIDAD,
                ESTADO_HABITACION = @ESTADO,
                ESTADO_ACTIVO_HABITACION = @ESTADO_ACTIVO,
                FECHA_MODIFICACION_HABITACION = @FECHA_MOD
            WHERE ID_HABITACION = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = idHabitacion;
        cmd.Parameters.AddWithValue("@TIPO", dto.IdTipoHabitacion);
        cmd.Parameters.AddWithValue("@CIUDAD", dto.IdCiudad);
        cmd.Parameters.AddWithValue("@HOTEL", dto.IdHotel);
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreHabitacion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PRECIO_NORMAL", (object?)dto.PrecioNormalHabitacion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PRECIO_ACTUAL", (object?)dto.PrecioActualHabitacion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CAPACIDAD", (object?)dto.CapacidadHabitacion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoHabitacion ?? true);
        cmd.Parameters.AddWithValue("@ESTADO_ACTIVO", dto.EstadoActivoHabitacion ?? true);
        cmd.Parameters.AddWithValue("@FECHA_MOD", now);

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0) return null;

        dto.IdHabitacion = idHabitacion;
        dto.FechaModificacionHabitacion = now;
        return dto;
    }

    public async Task<bool> EliminarAsync(string idHabitacion)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
UPDATE HABITACION
SET ESTADO_HABITACION = 0
WHERE ID_HABITACION = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.VarChar, 50).Value = idHabitacion;

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        return rows > 0;
    }

    private static HabitacionDto MapHabitacion(SqlDataReader dr) => new()
    {
        IdHabitacion = dr.GetString(dr.GetOrdinal("ID_HABITACION")),
        IdTipoHabitacion = dr.GetInt32(dr.GetOrdinal("ID_TIPO_HABITACION")),
        IdCiudad = dr.GetInt32(dr.GetOrdinal("ID_CIUDAD")),
        IdHotel = dr.GetInt32(dr.GetOrdinal("ID_HOTEL")),
        NombreHabitacion = dr.IsDBNull(dr.GetOrdinal("NOMBRE_HABITACION")) ? null : dr.GetString(dr.GetOrdinal("NOMBRE_HABITACION")),
        PrecioNormalHabitacion = dr.IsDBNull(dr.GetOrdinal("PRECIO_NORMAL_HABITACION")) ? null : dr.GetDecimal(dr.GetOrdinal("PRECIO_NORMAL_HABITACION")),
        PrecioActualHabitacion = dr.IsDBNull(dr.GetOrdinal("PRECIO_ACTUAL_HABITACION")) ? null : dr.GetDecimal(dr.GetOrdinal("PRECIO_ACTUAL_HABITACION")),
        CapacidadHabitacion = dr.IsDBNull(dr.GetOrdinal("CAPACIDAD_HABITACION")) ? null : dr.GetInt32(dr.GetOrdinal("CAPACIDAD_HABITACION")),
        EstadoHabitacion = dr.IsDBNull(dr.GetOrdinal("ESTADO_HABITACION")) ? null : dr.GetBoolean(dr.GetOrdinal("ESTADO_HABITACION")),
        FechaRegistroHabitacion = dr.IsDBNull(dr.GetOrdinal("FECHA_REGISTRO_HABITACION")) ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_REGISTRO_HABITACION")),
        EstadoActivoHabitacion = dr.IsDBNull(dr.GetOrdinal("ESTADO_ACTIVO_HABITACION")) ? null : dr.GetBoolean(dr.GetOrdinal("ESTADO_ACTIVO_HABITACION")),
        FechaModificacionHabitacion = dr.IsDBNull(dr.GetOrdinal("FECHA_MODIFICACION_HABITACION")) ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_MODIFICACION_HABITACION"))
    };
}
