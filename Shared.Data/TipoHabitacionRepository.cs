using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class TipoHabitacionRepository
{
    private readonly string _connectionString;

    public TipoHabitacionRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<TipoHabitacionDto>> ObtenerTodosAsync()
    {
        var lista = new List<TipoHabitacionDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_TIPO_HABITACION,
                   NOMBRE_HABITACION,
                   ESTADO_TIPO_HABITACION,
                   FECHA_MODIFICACION_TIPO_HABITACION
            FROM TIPO_HABITACION", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<TipoHabitacionDto?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_TIPO_HABITACION,
                   NOMBRE_HABITACION,
                   ESTADO_TIPO_HABITACION,
                   FECHA_MODIFICACION_TIPO_HABITACION
            FROM TIPO_HABITACION
            WHERE ID_TIPO_HABITACION = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<TipoHabitacionDto> CrearAsync(TipoHabitacionDto dto)
    {
        if (dto.IdTipoHabitacion <= 0)
            throw new ArgumentException("ID_TIPO_HABITACION es obligatorio");

        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO TIPO_HABITACION
            (
                ID_TIPO_HABITACION,
                NOMBRE_HABITACION,
                ESTADO_TIPO_HABITACION,
                FECHA_MODIFICACION_TIPO_HABITACION
            )
            VALUES
            (
                @ID,
                @NOMBRE,
                @ESTADO,
                @FECHA
            )", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = dto.IdTipoHabitacion;

        cmd.Parameters.Add("@NOMBRE", SqlDbType.VarChar, 250).Value =
            (object?)dto.NombreTipoHabitacion ?? DBNull.Value;

        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoTipoHabitacion ?? true;

        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        dto.FechaModificacionTipoHabitacion = now;
        return dto;
    }

    public async Task<TipoHabitacionDto?> ActualizarAsync(int id, TipoHabitacionDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE TIPO_HABITACION SET
                NOMBRE_HABITACION = @NOMBRE,
                ESTADO_TIPO_HABITACION = @ESTADO,
                FECHA_MODIFICACION_TIPO_HABITACION = @FECHA
            WHERE ID_TIPO_HABITACION = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

        cmd.Parameters.Add("@NOMBRE", SqlDbType.VarChar, 250).Value =
            (object?)dto.NombreTipoHabitacion ?? DBNull.Value;

        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoTipoHabitacion ?? true;

        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0) return null;

        dto.IdTipoHabitacion = id;
        dto.FechaModificacionTipoHabitacion = now;
        return dto;
    }

    // ✅ Eliminación lógica recomendada
    public async Task<bool> DesactivarAsync(int id)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE TIPO_HABITACION
            SET ESTADO_TIPO_HABITACION = 0,
                FECHA_MODIFICACION_TIPO_HABITACION = @FECHA
            WHERE ID_TIPO_HABITACION = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    // ❌ Eliminación física (solo si el negocio lo permite)
    public async Task<bool> EliminarFisicoAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"
UPDATE TIPO_HABITACION
SET ESTADO_TIPO_HABITACION = 0
WHERE ID_TIPO_HABITACION = @ID
", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static TipoHabitacionDto Map(SqlDataReader dr) => new()
    {
        IdTipoHabitacion = dr.GetInt32(dr.GetOrdinal("ID_TIPO_HABITACION")),
        NombreTipoHabitacion = dr.IsDBNull(dr.GetOrdinal("NOMBRE_HABITACION"))
            ? null : dr.GetString(dr.GetOrdinal("NOMBRE_HABITACION")),
        EstadoTipoHabitacion = dr.IsDBNull(dr.GetOrdinal("ESTADO_TIPO_HABITACION"))
            ? null : dr.GetBoolean(dr.GetOrdinal("ESTADO_TIPO_HABITACION")),
        FechaModificacionTipoHabitacion = dr.IsDBNull(dr.GetOrdinal("FECHA_MODIFICACION_TIPO_HABITACION"))
            ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_MODIFICACION_TIPO_HABITACION"))
    };
}
