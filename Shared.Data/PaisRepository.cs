using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class PaisRepository
{
    private readonly string _connectionString;

    public PaisRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<PaisDto>> ObtenerTodosAsync()
    {
        var lista = new List<PaisDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_PAIS, NOMBRE_PAIS, ESTADO_PAIS, FECHA_MODIFICACION_PAIS FROM PAIS", cn);
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync()) lista.Add(Map(dr));
        return lista;
    }

    public async Task<PaisDto?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_PAIS, NOMBRE_PAIS, ESTADO_PAIS, FECHA_MODIFICACION_PAIS FROM PAIS WHERE ID_PAIS = @ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<PaisDto> CrearAsync(PaisDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("INSERT INTO PAIS (ID_PAIS, NOMBRE_PAIS, ESTADO_PAIS, FECHA_MODIFICACION_PAIS) VALUES (@ID, @NOMBRE, @ESTADO, @FECHA)", cn);
        cmd.Parameters.AddWithValue("@ID", dto.IdPais);
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombrePais ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoPais ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        dto.FechaModificacionPais = now;
        return dto;
    }

    public async Task<PaisDto?> ActualizarAsync(int id, PaisDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("UPDATE PAIS SET NOMBRE_PAIS=@NOMBRE, ESTADO_PAIS=@ESTADO, FECHA_MODIFICACION_PAIS=@FECHA WHERE ID_PAIS=@ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombrePais ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoPais ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("UPDATE PAIS SET ESTADO_PAIS = 0 WHERE ID_PAIS = @ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static PaisDto Map(SqlDataReader dr) => new()
    {
        IdPais = dr.GetInt32(0),
        NombrePais = dr.IsDBNull(1) ? null : dr.GetString(1),
        EstadoPais = dr.IsDBNull(2) ? null : dr.GetBoolean(2),
        FechaModificacionPais = dr.IsDBNull(3) ? null : dr.GetDateTime(3)
    };
}
