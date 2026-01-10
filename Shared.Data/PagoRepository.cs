using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class PagoRepository
{
    private readonly string _connectionString;

    public PagoRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<PagoDto>> ObtenerTodosAsync()
    {
        var lista = new List<PagoDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_PAGO, ID_METODO_PAGO, ID_UNICO_USUARIO_EXTERNO, ID_UNICO_USUARIO,
                   ID_FACTURA, ID_RESERVA, CUENTA_ORIGEN_PAGO, CUENTA_DESTINO_PAGO,
                   MONTO_TOTAL_PAGO, ESTADO_PAGO, FECHA_MODIFICACION_PAGO
            FROM PAGO", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(MapPago(dr));

        return lista;
    }

    public async Task<PagoDto?> ObtenerPorIdAsync(int idPago)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_PAGO, ID_METODO_PAGO, ID_UNICO_USUARIO_EXTERNO, ID_UNICO_USUARIO,
                   ID_FACTURA, ID_RESERVA, CUENTA_ORIGEN_PAGO, CUENTA_DESTINO_PAGO,
                   MONTO_TOTAL_PAGO, ESTADO_PAGO, FECHA_MODIFICACION_PAGO
            FROM PAGO
            WHERE ID_PAGO = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idPago;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        if (await dr.ReadAsync())
            return MapPago(dr);

        return null;
    }

    public async Task<PagoDto> CrearAsync(PagoDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO PAGO
            (ID_PAGO, ID_METODO_PAGO, ID_UNICO_USUARIO_EXTERNO, ID_UNICO_USUARIO,
             ID_FACTURA, ID_RESERVA, CUENTA_ORIGEN_PAGO, CUENTA_DESTINO_PAGO,
             MONTO_TOTAL_PAGO, ESTADO_PAGO, FECHA_MODIFICACION_PAGO)
            VALUES (@ID, @METODO, @USUARIO_EXT, @USUARIO, @FACTURA, @RESERVA,
                    @CUENTA_ORIGEN, @CUENTA_DESTINO, @MONTO, @ESTADO, @FECHA_MOD)", cn);

        cmd.Parameters.AddWithValue("@ID", dto.IdPago);
        cmd.Parameters.AddWithValue("@METODO", dto.IdMetodoPago);
        cmd.Parameters.AddWithValue("@USUARIO_EXT", (object?)dto.IdUnicoUsuarioExterno ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@USUARIO", dto.IdUnicoUsuario);
        cmd.Parameters.AddWithValue("@FACTURA", (object?)dto.IdFactura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@RESERVA", (object?)dto.IdReserva ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CUENTA_ORIGEN", (object?)dto.CuentaOrigenPago ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CUENTA_DESTINO", (object?)dto.CuentaDestinoPago ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@MONTO", (object?)dto.MontoTotalPago ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoPago ?? true);
        cmd.Parameters.AddWithValue("@FECHA_MOD", now);

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        
        dto.FechaModificacionPago = now;
        return dto;
    }

    public async Task<PagoDto?> ActualizarAsync(int idPago, PagoDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE PAGO SET
                ID_METODO_PAGO = @METODO,
                ID_UNICO_USUARIO_EXTERNO = @USUARIO_EXT,
                ID_UNICO_USUARIO = @USUARIO,
                ID_FACTURA = @FACTURA,
                ID_RESERVA = @RESERVA,
                CUENTA_ORIGEN_PAGO = @CUENTA_ORIGEN,
                CUENTA_DESTINO_PAGO = @CUENTA_DESTINO,
                MONTO_TOTAL_PAGO = @MONTO,
                ESTADO_PAGO = @ESTADO,
                FECHA_MODIFICACION_PAGO = @FECHA_MOD
            WHERE ID_PAGO = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idPago;
        cmd.Parameters.AddWithValue("@METODO", dto.IdMetodoPago);
        cmd.Parameters.AddWithValue("@USUARIO_EXT", (object?)dto.IdUnicoUsuarioExterno ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@USUARIO", dto.IdUnicoUsuario);
        cmd.Parameters.AddWithValue("@FACTURA", (object?)dto.IdFactura ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@RESERVA", (object?)dto.IdReserva ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CUENTA_ORIGEN", (object?)dto.CuentaOrigenPago ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CUENTA_DESTINO", (object?)dto.CuentaDestinoPago ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@MONTO", (object?)dto.MontoTotalPago ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoPago ?? true);
        cmd.Parameters.AddWithValue("@FECHA_MOD", now);

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0) return null;

        dto.IdPago = idPago;
        dto.FechaModificacionPago = now;
        return dto;
    }

    public async Task<bool> EliminarAsync(int idPago)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
UPDATE PAGO
SET ESTADO_PAGO = 0
WHERE ID_PAGO = @ID
", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idPago;

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        return rows > 0;
    }

    private static PagoDto MapPago(SqlDataReader dr)
    {
        int Ord(string c) => dr.GetOrdinal(c);

        return new PagoDto
        {
            IdPago = dr.GetInt32(Ord("ID_PAGO")),
            IdMetodoPago = dr.GetInt32(Ord("ID_METODO_PAGO")),

            IdUnicoUsuarioExterno = dr.IsDBNull(Ord("ID_UNICO_USUARIO_EXTERNO"))
                ? null
                : dr.GetInt32(Ord("ID_UNICO_USUARIO_EXTERNO")),

            IdUnicoUsuario = dr.IsDBNull(Ord("ID_UNICO_USUARIO"))
                ? null
                : dr.GetInt32(Ord("ID_UNICO_USUARIO")),

            IdFactura = dr.IsDBNull(Ord("ID_FACTURA"))
                ? null
                : dr.GetInt32(Ord("ID_FACTURA")),

            IdReserva = dr.IsDBNull(Ord("ID_RESERVA"))
                ? null
                : dr.GetInt32(Ord("ID_RESERVA")),

            CuentaOrigenPago = dr.IsDBNull(Ord("CUENTA_ORIGEN_PAGO"))
                ? null
                : dr.GetString(Ord("CUENTA_ORIGEN_PAGO")),

            CuentaDestinoPago = dr.IsDBNull(Ord("CUENTA_DESTINO_PAGO"))
                ? null
                : dr.GetString(Ord("CUENTA_DESTINO_PAGO")),

            MontoTotalPago = dr.IsDBNull(Ord("MONTO_TOTAL_PAGO"))
                ? null
                : dr.GetDecimal(Ord("MONTO_TOTAL_PAGO")),

            EstadoPago = dr.IsDBNull(Ord("ESTADO_PAGO"))
                ? null
                : dr.GetBoolean(Ord("ESTADO_PAGO")),

            FechaModificacionPago = dr.IsDBNull(Ord("FECHA_MODIFICACION_PAGO"))
                ? null
                : dr.GetDateTime(Ord("FECHA_MODIFICACION_PAGO"))
        };
    }

    
}
