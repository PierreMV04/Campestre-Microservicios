using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class RolRepository
{
    private readonly string _connectionString;

    public RolRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<RolDto>> ObtenerTodosAsync()
    {
        var lista = new List<RolDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_ROL, NOMBRE_ROL, ESTADO_ROL, FECHA_MODIFICACION_ROL
            FROM ROL", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<RolDto?> ObtenerPorIdAsync(int idRol)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_ROL, NOMBRE_ROL, ESTADO_ROL, FECHA_MODIFICACION_ROL
            FROM ROL
            WHERE ID_ROL = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idRol;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<RolDto> CrearAsync(RolDto dto)
    {
        if (dto.IdRol <= 0)
            throw new ArgumentException("ID_ROL es obligatorio");

        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO ROL
            (
                ID_ROL,
                NOMBRE_ROL,
                ESTADO_ROL,
                FECHA_MODIFICACION_ROL
            )
            VALUES
            (
                @ID,
                @NOMBRE,
                @ESTADO,
                @FECHA
            )", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = dto.IdRol;
        cmd.Parameters.Add("@NOMBRE", SqlDbType.VarChar, 100).Value =
            (object?)dto.NombreRol ?? DBNull.Value;

        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoRol ?? true;

        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        dto.FechaModificacionRol = now;
        return dto;
    }

    public async Task<RolDto?> ActualizarAsync(int idRol, RolDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE ROL SET
                NOMBRE_ROL = @NOMBRE,
                ESTADO_ROL = @ESTADO,
                FECHA_MODIFICACION_ROL = @FECHA
            WHERE ID_ROL = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idRol;
        cmd.Parameters.Add("@NOMBRE", SqlDbType.VarChar, 100).Value =
            (object?)dto.NombreRol ?? DBNull.Value;

        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoRol ?? true;

        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0) return null;

        dto.IdRol = idRol;
        dto.FechaModificacionRol = now;
        return dto;
    }

    // ✅ Eliminación lógica (RECOMENDADA)
    public async Task<bool> DesactivarAsync(int idRol)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE ROL
            SET ESTADO_ROL = 0,
                FECHA_MODIFICACION_ROL = @FECHA
            WHERE ID_ROL = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idRol;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    // ❌ Eliminación física (solo si el negocio lo permite)
    public async Task<bool> EliminarFisicoAsync(int idRol)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"
UPDATE ROL
SET ESTADO_ROL = 0
WHERE ID_ROL = @ID
", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idRol;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static RolDto Map(SqlDataReader dr) => new()
    {
        IdRol = dr.GetInt32(dr.GetOrdinal("ID_ROL")),
        NombreRol = dr.IsDBNull(dr.GetOrdinal("NOMBRE_ROL"))
            ? null : dr.GetString(dr.GetOrdinal("NOMBRE_ROL")),
        EstadoRol = dr.IsDBNull(dr.GetOrdinal("ESTADO_ROL"))
            ? null : dr.GetBoolean(dr.GetOrdinal("ESTADO_ROL")),
        FechaModificacionRol = dr.IsDBNull(dr.GetOrdinal("FECHA_MODIFICACION_ROL"))
            ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_MODIFICACION_ROL"))
    };
}
