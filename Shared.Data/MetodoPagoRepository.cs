using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class MetodoPagoRepository
{
    private readonly string _connectionString;

    public MetodoPagoRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<MetodoPagoDto>> ObtenerTodosAsync()
    {
        var lista = new List<MetodoPagoDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_METODO_PAGO, NOMBRE_METODO_PAGO, ESTADO_METODO_PAGO, FECHA_MODIFICACION_METODO_PAGO FROM METODO_PAGO", cn);
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync()) lista.Add(Map(dr));
        return lista;
    }

    public async Task<MetodoPagoDto?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_METODO_PAGO, NOMBRE_METODO_PAGO, ESTADO_METODO_PAGO, FECHA_MODIFICACION_METODO_PAGO FROM METODO_PAGO WHERE ID_METODO_PAGO = @ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<MetodoPagoDto> CrearAsync(MetodoPagoDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("INSERT INTO METODO_PAGO (ID_METODO_PAGO, NOMBRE_METODO_PAGO, ESTADO_METODO_PAGO, FECHA_MODIFICACION_METODO_PAGO) VALUES (@ID, @NOMBRE, @ESTADO, @FECHA)", cn);
        cmd.Parameters.AddWithValue("@ID", dto.IdMetodoPago);
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreMetodoPago ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoMetodoPago ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        dto.FechaModificacionMetodoPago = now;
        return dto;
    }

    public async Task<MetodoPagoDto?> ActualizarAsync(int id, MetodoPagoDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("UPDATE METODO_PAGO SET NOMBRE_METODO_PAGO=@NOMBRE, ESTADO_METODO_PAGO=@ESTADO, FECHA_MODIFICACION_METODO_PAGO=@FECHA WHERE ID_METODO_PAGO=@ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreMetodoPago ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoMetodoPago ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
UPDATE METODO_PAGO
SET ESTADO_METODO_PAGO = 0
WHERE ID_METODO_PAGO = @ID
", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static MetodoPagoDto Map(SqlDataReader dr) => new()
    {
        IdMetodoPago = dr.GetInt32(0),
        NombreMetodoPago = dr.IsDBNull(1) ? null : dr.GetString(1),
        EstadoMetodoPago = dr.IsDBNull(2) ? null : dr.GetBoolean(2),
        FechaModificacionMetodoPago = dr.IsDBNull(3) ? null : dr.GetDateTime(3)
    };
}
