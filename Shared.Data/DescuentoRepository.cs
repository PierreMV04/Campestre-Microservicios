using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class DescuentoRepository
{
    private readonly string _connectionString;

    public DescuentoRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<DescuentoDto>> ObtenerTodosAsync()
    {
        var lista = new List<DescuentoDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"SELECT ID_DESCUENTO, NOMBRE_DESCUENTO, VALOR_DESCUENTO,
                     FECHA_INICIO_DESCUENTO, FECHA_FIN_DESCUENTO,
                     ESTADO_DESCUENTO, FECHA_MODIFICACION_DESCUENTO
              FROM DESCUENTO", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<DescuentoDto?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"SELECT ID_DESCUENTO, NOMBRE_DESCUENTO, VALOR_DESCUENTO,
                     FECHA_INICIO_DESCUENTO, FECHA_FIN_DESCUENTO,
                     ESTADO_DESCUENTO, FECHA_MODIFICACION_DESCUENTO
              FROM DESCUENTO WHERE ID_DESCUENTO = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<DescuentoDto> CrearAsync(DescuentoDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"INSERT INTO DESCUENTO
              (ID_DESCUENTO, NOMBRE_DESCUENTO, VALOR_DESCUENTO,
               FECHA_INICIO_DESCUENTO, FECHA_FIN_DESCUENTO,
               ESTADO_DESCUENTO, FECHA_MODIFICACION_DESCUENTO)
              VALUES (@ID, @NOMBRE, @VALOR, @INICIO, @FIN, @ESTADO, @FECHA)", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = dto.IdDescuento;
        cmd.Parameters.Add("@NOMBRE", SqlDbType.VarChar, 200).Value =
            (object?)dto.NombreDescuento ?? DBNull.Value;
        cmd.Parameters.Add("@VALOR", SqlDbType.Decimal).Value =
            (object?)dto.ValorDescuento ?? DBNull.Value;
        cmd.Parameters.Add("@INICIO", SqlDbType.DateTime).Value =
            (object?)dto.FechaInicioDescuento ?? DBNull.Value;
        cmd.Parameters.Add("@FIN", SqlDbType.DateTime).Value =
            (object?)dto.FechaFinDescuento ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoDescuento ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        dto.FechaModificacionDescuento = now;
        return dto;
    }

    public async Task<DescuentoDto?> ActualizarAsync(int id, DescuentoDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"UPDATE DESCUENTO SET
                NOMBRE_DESCUENTO = @NOMBRE,
                VALOR_DESCUENTO = @VALOR,
                FECHA_INICIO_DESCUENTO = @INICIO,
                FECHA_FIN_DESCUENTO = @FIN,
                ESTADO_DESCUENTO = @ESTADO,
                FECHA_MODIFICACION_DESCUENTO = @FECHA
              WHERE ID_DESCUENTO = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        cmd.Parameters.Add("@NOMBRE", SqlDbType.VarChar, 200).Value =
            (object?)dto.NombreDescuento ?? DBNull.Value;
        cmd.Parameters.Add("@VALOR", SqlDbType.Decimal).Value =
            (object?)dto.ValorDescuento ?? DBNull.Value;
        cmd.Parameters.Add("@INICIO", SqlDbType.DateTime).Value =
            (object?)dto.FechaInicioDescuento ?? DBNull.Value;
        cmd.Parameters.Add("@FIN", SqlDbType.DateTime).Value =
            (object?)dto.FechaFinDescuento ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoDescuento ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            "UPDATE DESCUENTO SET ESTADO_DESCUENTO = 0 WHERE ID_DESCUENTO = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static DescuentoDto Map(SqlDataReader dr) => new()
    {
        IdDescuento = dr.GetInt32(0),
        NombreDescuento = dr.IsDBNull(1) ? null : dr.GetString(1),
        ValorDescuento = dr.IsDBNull(2) ? null : dr.GetDecimal(2),
        FechaInicioDescuento = dr.IsDBNull(3) ? null : dr.GetDateTime(3),
        FechaFinDescuento = dr.IsDBNull(4) ? null : dr.GetDateTime(4),
        EstadoDescuento = dr.IsDBNull(5) ? null : dr.GetBoolean(5),
        FechaModificacionDescuento = dr.IsDBNull(6) ? null : dr.GetDateTime(6)
    };
}
