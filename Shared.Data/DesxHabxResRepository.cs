using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class DesxHabxResRepository
{
    private readonly string _connectionString;

    public DesxHabxResRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<DesxHabxResDto>> ObtenerTodosAsync()
    {
        var lista = new List<DesxHabxResDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"SELECT ID_DESCUENTO, ID_HABXRES, MONTO_DESXHABXRES,
                     ESTADO_DESXHABXRES, FECHA_MODIFICACION_DESXHABXRES
              FROM DESXHABXRES", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<DesxHabxResDto?> ObtenerPorIdAsync(int idDescuento, int idHabxRes)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"SELECT ID_DESCUENTO, ID_HABXRES, MONTO_DESXHABXRES,
                     ESTADO_DESXHABXRES, FECHA_MODIFICACION_DESXHABXRES
              FROM DESXHABXRES
              WHERE ID_DESCUENTO = @ID_DES AND ID_HABXRES = @ID_HABXRES", cn);

        cmd.Parameters.Add("@ID_DES", SqlDbType.Int).Value = idDescuento;
        cmd.Parameters.Add("@ID_HABXRES", SqlDbType.Int).Value = idHabxRes;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<DesxHabxResDto> CrearAsync(DesxHabxResDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"INSERT INTO DESXHABXRES
              (ID_DESCUENTO, ID_HABXRES, MONTO_DESXHABXRES,
               ESTADO_DESXHABXRES, FECHA_MODIFICACION_DESXHABXRES)
              VALUES (@ID_DES, @ID_HABXRES, @MONTO, @ESTADO, @FECHA)", cn);

        cmd.Parameters.Add("@ID_DES", SqlDbType.Int).Value = dto.IdDescuento;
        cmd.Parameters.Add("@ID_HABXRES", SqlDbType.Int).Value = dto.IdHabxRes;
        cmd.Parameters.Add("@MONTO", SqlDbType.Decimal).Value =
            (object?)dto.MontoDesxHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoDesxHabxRes ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        dto.FechaModificacionDesxHabxRes = now;
        return dto;
    }

    public async Task<DesxHabxResDto?> ActualizarAsync(int idDescuento, int idHabxRes, DesxHabxResDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"UPDATE DESXHABXRES SET
                MONTO_DESXHABXRES = @MONTO,
                ESTADO_DESXHABXRES = @ESTADO,
                FECHA_MODIFICACION_DESXHABXRES = @FECHA
              WHERE ID_DESCUENTO = @ID_DES AND ID_HABXRES = @ID_HABXRES", cn);

        cmd.Parameters.Add("@ID_DES", SqlDbType.Int).Value = idDescuento;
        cmd.Parameters.Add("@ID_HABXRES", SqlDbType.Int).Value = idHabxRes;
        cmd.Parameters.Add("@MONTO", SqlDbType.Decimal).Value =
            (object?)dto.MontoDesxHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value =
            dto.EstadoDesxHabxRes ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(int idDescuento, int idHabxRes)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"UPDATE DESXHABXRES
SET ESTADO_DESXHABXRES = 0
WHERE ID_DESCUENTO = @ID_DES AND ID_HABXRES = @ID_HABXRES
", cn);

        cmd.Parameters.Add("@ID_DES", SqlDbType.Int).Value = idDescuento;
        cmd.Parameters.Add("@ID_HABXRES", SqlDbType.Int).Value = idHabxRes;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static DesxHabxResDto Map(SqlDataReader dr) => new()
    {
        IdDescuento = dr.GetInt32(0),
        IdHabxRes = dr.GetInt32(1),
        MontoDesxHabxRes = dr.IsDBNull(2) ? null : dr.GetDecimal(2),
        EstadoDesxHabxRes = dr.IsDBNull(3) ? null : dr.GetBoolean(3),
        FechaModificacionDesxHabxRes = dr.IsDBNull(4) ? null : dr.GetDateTime(4)
    };
    public async Task<List<DesxHabxResDto>> ObtenerPorHabxResAsync(int idHabxRes)
    {
        var lista = new List<DesxHabxResDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
        SELECT ID_DESCUENTO, ID_HABXRES, MONTO_DESXHABXRES,
               ESTADO_DESXHABXRES, FECHA_MODIFICACION_DESXHABXRES
        FROM DESXHABXRES
        WHERE ID_HABXRES = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idHabxRes;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

}
