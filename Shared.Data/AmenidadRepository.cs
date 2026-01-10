using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class AmenidadRepository
{
    private readonly string _connectionString;

    public AmenidadRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<AmenidadDto>> ObtenerTodosAsync()
    {
        var lista = new List<AmenidadDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_AMENIDAD, NOMBRE_AMENIDAD, ESTADO_AMENIDAD, FECHA_MODIFICACION_AMENIDAD FROM AMENIDAD", cn);
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync()) lista.Add(Map(dr));
        return lista;
    }

    public async Task<AmenidadDto?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_AMENIDAD, NOMBRE_AMENIDAD, ESTADO_AMENIDAD, FECHA_MODIFICACION_AMENIDAD FROM AMENIDAD WHERE ID_AMENIDAD = @ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<AmenidadDto> CrearAsync(AmenidadDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("INSERT INTO AMENIDAD (ID_AMENIDAD, NOMBRE_AMENIDAD, ESTADO_AMENIDAD, FECHA_MODIFICACION_AMENIDAD) VALUES (@ID, @NOMBRE, @ESTADO, @FECHA)", cn);
        cmd.Parameters.AddWithValue("@ID", dto.IdAmenidad);
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreAmenidad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoAmenidad ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        dto.FechaModificacionAmenidad = now;
        return dto;
    }

    public async Task<AmenidadDto?> ActualizarAsync(int id, AmenidadDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("UPDATE AMENIDAD SET NOMBRE_AMENIDAD=@NOMBRE, ESTADO_AMENIDAD=@ESTADO, FECHA_MODIFICACION_AMENIDAD=@FECHA WHERE ID_AMENIDAD=@ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreAmenidad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoAmenidad ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("UPDATE AMENIDAD SET ESTADO_AMENIDAD = 0 WHERE ID_AMENIDAD = @ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static AmenidadDto Map(SqlDataReader dr) => new()
    {
        IdAmenidad = dr.GetInt32(0),
        NombreAmenidad = dr.IsDBNull(1) ? null : dr.GetString(1),
        EstadoAmenidad = dr.IsDBNull(2) ? null : dr.GetBoolean(2),
        FechaModificacionAmenidad = dr.IsDBNull(3) ? null : dr.GetDateTime(3)
    };
}
