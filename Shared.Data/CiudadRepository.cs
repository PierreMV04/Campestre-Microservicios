using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class CiudadRepository
{
    private readonly string _connectionString;

    public CiudadRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<CiudadDto>> ObtenerTodosAsync()
    {
        var lista = new List<CiudadDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_CIUDAD, ID_PAIS, NOMBRE_CIUDAD, ESTADO_CIUDAD, FECHA_MODIFICACION_CIUDAD FROM CIUDAD", cn);
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync()) lista.Add(Map(dr));
        return lista;
    }

    public async Task<CiudadDto?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_CIUDAD, ID_PAIS, NOMBRE_CIUDAD, ESTADO_CIUDAD, FECHA_MODIFICACION_CIUDAD FROM CIUDAD WHERE ID_CIUDAD = @ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<CiudadDto> CrearAsync(CiudadDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("INSERT INTO CIUDAD (ID_CIUDAD, ID_PAIS, NOMBRE_CIUDAD, ESTADO_CIUDAD, FECHA_MODIFICACION_CIUDAD) VALUES (@ID, @PAIS, @NOMBRE, @ESTADO, @FECHA)", cn);
        cmd.Parameters.AddWithValue("@ID", dto.IdCiudad);
        cmd.Parameters.AddWithValue("@PAIS", dto.IdPais);
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreCiudad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoCiudad ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        dto.FechaModificacionCiudad = now;
        return dto;
    }

    public async Task<CiudadDto?> ActualizarAsync(int id, CiudadDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("UPDATE CIUDAD SET ID_PAIS=@PAIS, NOMBRE_CIUDAD=@NOMBRE, ESTADO_CIUDAD=@ESTADO, FECHA_MODIFICACION_CIUDAD=@FECHA WHERE ID_CIUDAD=@ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        cmd.Parameters.AddWithValue("@PAIS", dto.IdPais);
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreCiudad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoCiudad ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("UPDATE CIUDAD SET ESTADO_CIUDAD = 0 WHERE ID_CIUDAD = @ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static CiudadDto Map(SqlDataReader dr) => new()
    {
        IdCiudad = dr.GetInt32(0),
        IdPais = dr.GetInt32(1),
        NombreCiudad = dr.IsDBNull(2) ? null : dr.GetString(2),
        EstadoCiudad = dr.IsDBNull(3) ? null : dr.GetBoolean(3),
        FechaModificacionCiudad = dr.IsDBNull(4) ? null : dr.GetDateTime(4)
    };
}
