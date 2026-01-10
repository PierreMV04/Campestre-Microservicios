using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class ReservaRepository
{
    private readonly string _connectionString;

    public ReservaRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<ReservaDto>> ObtenerTodasAsync()
    {
        var lista = new List<ReservaDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_RESERVA, ID_UNICO_USUARIO, ID_UNICO_USUARIO_EXTERNO,
                   COSTO_TOTAL_RESERVA, FECHA_REGISTRO_RESERVA,
                   FECHA_INICIO_RESERVA, FECHA_FINAL_RESERVA,
                   ESTADO_GENERAL_RESERVA, ESTADO_RESERVA,
                   FECHA_MODIFICACION_RESERVA
            FROM RESERVA", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<ReservaDto?> ObtenerPorIdAsync(int idReserva)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_RESERVA, ID_UNICO_USUARIO, ID_UNICO_USUARIO_EXTERNO,
                   COSTO_TOTAL_RESERVA, FECHA_REGISTRO_RESERVA,
                   FECHA_INICIO_RESERVA, FECHA_FINAL_RESERVA,
                   ESTADO_GENERAL_RESERVA, ESTADO_RESERVA,
                   FECHA_MODIFICACION_RESERVA
            FROM RESERVA
            WHERE ID_RESERVA = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idReserva;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<ReservaDto> CrearAsync(ReservaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EstadoGeneralReserva))
            throw new ArgumentException("ESTADO_GENERAL_RESERVA es obligatorio");

        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO RESERVA
            (
                ID_RESERVA,
                ID_UNICO_USUARIO,
                ID_UNICO_USUARIO_EXTERNO,
                COSTO_TOTAL_RESERVA,
                FECHA_REGISTRO_RESERVA,
                FECHA_INICIO_RESERVA,
                FECHA_FINAL_RESERVA,
                ESTADO_GENERAL_RESERVA,
                ESTADO_RESERVA,
                FECHA_MODIFICACION_RESERVA
            )
            VALUES
            (
                @ID,
                @USUARIO,
                @USUARIO_EXT,
                @COSTO,
                @FECHA_REG,
                @FECHA_INI,
                @FECHA_FIN,
                @ESTADO_GEN,
                @ESTADO,
                @FECHA_MOD
            )", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = dto.IdReserva;
        cmd.Parameters.Add("@USUARIO", SqlDbType.Int).Value =
            (object?)dto.IdUnicoUsuario ?? DBNull.Value;

        cmd.Parameters.Add("@USUARIO_EXT", SqlDbType.Int).Value =
            (object?)dto.IdUnicoUsuarioExterno ?? DBNull.Value;

        cmd.Parameters.Add("@COSTO", SqlDbType.Decimal).Value =
            (object?)dto.CostoTotalReserva ?? DBNull.Value;

        cmd.Parameters.Add("@FECHA_REG", SqlDbType.DateTime).Value = now;

        cmd.Parameters.Add("@FECHA_INI", SqlDbType.DateTime).Value =
            (object?)dto.FechaInicioReserva ?? DBNull.Value;

        cmd.Parameters.Add("@FECHA_FIN", SqlDbType.DateTime).Value =
            (object?)dto.FechaFinalReserva ?? DBNull.Value;

        cmd.Parameters.Add("@ESTADO_GEN", SqlDbType.VarChar, 15).Value =
            dto.EstadoGeneralReserva;

        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoReserva ?? true;

        cmd.Parameters.Add("@FECHA_MOD", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        dto.FechaRegistroReserva = now;
        dto.FechaModificacionReserva = now;
        return dto;
    }

    public async Task<ReservaDto?> ActualizarAsync(int idReserva, ReservaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.EstadoGeneralReserva))
            throw new ArgumentException("ESTADO_GENERAL_RESERVA es obligatorio");

        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE RESERVA SET
                ID_UNICO_USUARIO = @USUARIO,
                ID_UNICO_USUARIO_EXTERNO = @USUARIO_EXT,
                COSTO_TOTAL_RESERVA = @COSTO,
                FECHA_INICIO_RESERVA = @FECHA_INI,
                FECHA_FINAL_RESERVA = @FECHA_FIN,
                ESTADO_GENERAL_RESERVA = @ESTADO_GEN,
                ESTADO_RESERVA = @ESTADO,
                FECHA_MODIFICACION_RESERVA = @FECHA_MOD
            WHERE ID_RESERVA = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idReserva;
        cmd.Parameters.Add("@USUARIO", SqlDbType.Int).Value =
            (object?)dto.IdUnicoUsuario ?? DBNull.Value;

        cmd.Parameters.Add("@USUARIO_EXT", SqlDbType.Int).Value =
            (object?)dto.IdUnicoUsuarioExterno ?? DBNull.Value;

        cmd.Parameters.Add("@COSTO", SqlDbType.Decimal).Value =
            (object?)dto.CostoTotalReserva ?? DBNull.Value;

        cmd.Parameters.Add("@FECHA_INI", SqlDbType.DateTime).Value =
            (object?)dto.FechaInicioReserva ?? DBNull.Value;

        cmd.Parameters.Add("@FECHA_FIN", SqlDbType.DateTime).Value =
            (object?)dto.FechaFinalReserva ?? DBNull.Value;

        cmd.Parameters.Add("@ESTADO_GEN", SqlDbType.VarChar, 15).Value =
            dto.EstadoGeneralReserva;

        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoReserva ?? true;

        cmd.Parameters.Add("@FECHA_MOD", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0) return null;

        dto.IdReserva = idReserva;
        dto.FechaModificacionReserva = now;
        return dto;
    }

    // ❗ Eliminación lógica: CANCELAR reserva
    public async Task<bool> CancelarAsync(int idReserva)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE RESERVA
            SET ESTADO_GENERAL_RESERVA = 'CANCELADO',
                ESTADO_RESERVA = 0,
                FECHA_MODIFICACION_RESERVA = @FECHA
            WHERE ID_RESERVA = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idReserva;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static ReservaDto Map(SqlDataReader dr) => new()
    {
        IdReserva = dr.GetInt32(dr.GetOrdinal("ID_RESERVA")),
        IdUnicoUsuario = dr.IsDBNull(dr.GetOrdinal("ID_UNICO_USUARIO"))
            ? null : dr.GetInt32(dr.GetOrdinal("ID_UNICO_USUARIO")),
        IdUnicoUsuarioExterno = dr.IsDBNull(dr.GetOrdinal("ID_UNICO_USUARIO_EXTERNO"))
            ? null : dr.GetInt32(dr.GetOrdinal("ID_UNICO_USUARIO_EXTERNO")),
        CostoTotalReserva = dr.IsDBNull(dr.GetOrdinal("COSTO_TOTAL_RESERVA"))
            ? null : dr.GetDecimal(dr.GetOrdinal("COSTO_TOTAL_RESERVA")),
        FechaRegistroReserva = dr.IsDBNull(dr.GetOrdinal("FECHA_REGISTRO_RESERVA"))
            ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_REGISTRO_RESERVA")),
        FechaInicioReserva = dr.IsDBNull(dr.GetOrdinal("FECHA_INICIO_RESERVA"))
            ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_INICIO_RESERVA")),
        FechaFinalReserva = dr.IsDBNull(dr.GetOrdinal("FECHA_FINAL_RESERVA"))
            ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_FINAL_RESERVA")),
        EstadoGeneralReserva = dr.GetString(dr.GetOrdinal("ESTADO_GENERAL_RESERVA")),
        EstadoReserva = dr.IsDBNull(dr.GetOrdinal("ESTADO_RESERVA"))
            ? null : dr.GetBoolean(dr.GetOrdinal("ESTADO_RESERVA")),
        FechaModificacionReserva = dr.IsDBNull(dr.GetOrdinal("FECHA_MODIFICACION_RESERVA"))
            ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_MODIFICACION_RESERVA"))
    };
}
