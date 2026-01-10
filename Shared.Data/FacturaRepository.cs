using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class FacturaRepository
{
    private readonly string _connectionString;

    public FacturaRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<FacturaDto>> ObtenerTodasAsync()
    {
        var lista = new List<FacturaDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_FACTURA, ID_RESERVA, ID_UNICO_USUARIO_EXTERNO, ID_UNICO_USUARIO,
                   SUBTOTAL_FACTURA, DESCUENTO_TOTAL_FACTURA, IMPUESTO_TOTAL_FACTURA,
                   FECHA_CREACION_FACTURA, EMAIL_USUARIO_EXTERNO, EMAIL_USUARIO,
                   ESTADO_FACTURA, FECHA_MODIFICACION_FACTURA
            FROM FACTURA", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(MapFactura(dr));

        return lista;
    }

    public async Task<FacturaDto?> ObtenerPorIdAsync(int idFactura)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_FACTURA, ID_RESERVA, ID_UNICO_USUARIO_EXTERNO, ID_UNICO_USUARIO,
                   SUBTOTAL_FACTURA, DESCUENTO_TOTAL_FACTURA, IMPUESTO_TOTAL_FACTURA,
                   FECHA_CREACION_FACTURA, EMAIL_USUARIO_EXTERNO, EMAIL_USUARIO,
                   ESTADO_FACTURA, FECHA_MODIFICACION_FACTURA
            FROM FACTURA
            WHERE ID_FACTURA = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idFactura;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        if (await dr.ReadAsync())
            return MapFactura(dr);

        return null;
    }

    public async Task<FacturaDto> CrearAsync(FacturaDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO FACTURA
            (ID_FACTURA, ID_RESERVA, ID_UNICO_USUARIO_EXTERNO, ID_UNICO_USUARIO,
             SUBTOTAL_FACTURA, DESCUENTO_TOTAL_FACTURA, IMPUESTO_TOTAL_FACTURA,
             FECHA_CREACION_FACTURA, EMAIL_USUARIO_EXTERNO, EMAIL_USUARIO,
             ESTADO_FACTURA, FECHA_MODIFICACION_FACTURA)
            VALUES (@ID, @RESERVA, @USUARIO_EXT, @USUARIO, @SUBTOTAL, @DESCUENTO,
                    @IMPUESTO, @FECHA_CREACION, @EMAIL_EXT, @EMAIL, @ESTADO, @FECHA_MOD)", cn);

        cmd.Parameters.AddWithValue("@ID", dto.IdFactura);
        cmd.Parameters.AddWithValue("@RESERVA", dto.IdReserva);
        cmd.Parameters.AddWithValue("@USUARIO_EXT", (object?)dto.IdUnicoUsuarioExterno ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@USUARIO", (object?)dto.IdUnicoUsuario ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SUBTOTAL", (object?)dto.SubtotalFactura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DESCUENTO", (object?)dto.DescuentoTotalFactura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IMPUESTO", (object?)dto.ImpuestoTotalFactura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FECHA_CREACION", now);
        cmd.Parameters.AddWithValue("@EMAIL_EXT", (object?)dto.EmailUsuarioExterno ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EMAIL", (object?)dto.EmailUsuario ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoFactura ?? true);
        cmd.Parameters.AddWithValue("@FECHA_MOD", now);

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        dto.FechaCreacionFactura = now;
        dto.FechaModificacionFactura = now;
        return dto;
    }

    public async Task<FacturaDto?> ActualizarAsync(int idFactura, FacturaDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE FACTURA SET
                ID_RESERVA = @RESERVA,
                ID_UNICO_USUARIO_EXTERNO = @USUARIO_EXT,
                ID_UNICO_USUARIO = @USUARIO,
                SUBTOTAL_FACTURA = @SUBTOTAL,
                DESCUENTO_TOTAL_FACTURA = @DESCUENTO,
                IMPUESTO_TOTAL_FACTURA = @IMPUESTO,
                EMAIL_USUARIO_EXTERNO = @EMAIL_EXT,
                EMAIL_USUARIO = @EMAIL,
                ESTADO_FACTURA = @ESTADO,
                FECHA_MODIFICACION_FACTURA = @FECHA_MOD
            WHERE ID_FACTURA = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idFactura;
        cmd.Parameters.AddWithValue("@RESERVA", dto.IdReserva);
        cmd.Parameters.AddWithValue("@USUARIO_EXT", (object?)dto.IdUnicoUsuarioExterno ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@USUARIO", (object?)dto.IdUnicoUsuario ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SUBTOTAL", (object?)dto.SubtotalFactura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DESCUENTO", (object?)dto.DescuentoTotalFactura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IMPUESTO", (object?)dto.ImpuestoTotalFactura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EMAIL_EXT", (object?)dto.EmailUsuarioExterno ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EMAIL", (object?)dto.EmailUsuario ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoFactura ?? true);
        cmd.Parameters.AddWithValue("@FECHA_MOD", now);

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0) return null;

        dto.IdFactura = idFactura;
        dto.FechaModificacionFactura = now;
        return dto;
    }

    public async Task<bool> EliminarAsync(int idFactura)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
UPDATE FACTURA
SET ESTADO_FACTURA = 0
WHERE ID_FACTURA = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idFactura;

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        return rows > 0;
    }

    private static FacturaDto MapFactura(SqlDataReader dr) => new()
    {
        IdFactura = dr.GetInt32(dr.GetOrdinal("ID_FACTURA")),
        IdReserva = dr.GetInt32(dr.GetOrdinal("ID_RESERVA")),
        IdUnicoUsuarioExterno = dr.IsDBNull(dr.GetOrdinal("ID_UNICO_USUARIO_EXTERNO")) ? null : dr.GetInt32(dr.GetOrdinal("ID_UNICO_USUARIO_EXTERNO")),
        IdUnicoUsuario = dr.IsDBNull(dr.GetOrdinal("ID_UNICO_USUARIO")) ? null : dr.GetInt32(dr.GetOrdinal("ID_UNICO_USUARIO")),
        SubtotalFactura = dr.IsDBNull(dr.GetOrdinal("SUBTOTAL_FACTURA")) ? null : dr.GetDecimal(dr.GetOrdinal("SUBTOTAL_FACTURA")),
        DescuentoTotalFactura = dr.IsDBNull(dr.GetOrdinal("DESCUENTO_TOTAL_FACTURA")) ? null : dr.GetDecimal(dr.GetOrdinal("DESCUENTO_TOTAL_FACTURA")),
        ImpuestoTotalFactura = dr.IsDBNull(dr.GetOrdinal("IMPUESTO_TOTAL_FACTURA")) ? null : dr.GetDecimal(dr.GetOrdinal("IMPUESTO_TOTAL_FACTURA")),
        FechaCreacionFactura = dr.IsDBNull(dr.GetOrdinal("FECHA_CREACION_FACTURA")) ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_CREACION_FACTURA")),
        EmailUsuarioExterno = dr.IsDBNull(dr.GetOrdinal("EMAIL_USUARIO_EXTERNO")) ? null : dr.GetString(dr.GetOrdinal("EMAIL_USUARIO_EXTERNO")),
        EmailUsuario = dr.IsDBNull(dr.GetOrdinal("EMAIL_USUARIO")) ? null : dr.GetString(dr.GetOrdinal("EMAIL_USUARIO")),
        EstadoFactura = dr.IsDBNull(dr.GetOrdinal("ESTADO_FACTURA")) ? null : dr.GetBoolean(dr.GetOrdinal("ESTADO_FACTURA")),
        FechaModificacionFactura = dr.IsDBNull(dr.GetOrdinal("FECHA_MODIFICACION_FACTURA")) ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_MODIFICACION_FACTURA"))
    };
}
