using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class HotelRepository
{
    private readonly string _connectionString;

    public HotelRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<HotelDto>> ObtenerTodosAsync()
    {
        var lista = new List<HotelDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HOTEL, NOMBRE_HOTEL, ESTADO_HOTEL, FECHA_MODIFICACION_HOTEL
            FROM HOTEL", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(MapHotel(dr));

        return lista;
    }

    public async Task<HotelDto?> ObtenerPorIdAsync(int idHotel)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HOTEL, NOMBRE_HOTEL, ESTADO_HOTEL, FECHA_MODIFICACION_HOTEL
            FROM HOTEL
            WHERE ID_HOTEL = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idHotel;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        if (await dr.ReadAsync())
            return MapHotel(dr);

        return null;
    }

    public async Task<HotelDto> CrearAsync(HotelDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO HOTEL
            (ID_HOTEL, NOMBRE_HOTEL, ESTADO_HOTEL, FECHA_MODIFICACION_HOTEL)
            VALUES (@ID, @NOMBRE, @ESTADO, @FECHA)", cn);

        cmd.Parameters.AddWithValue("@ID", dto.IdHotel);
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreHotel ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoHotel ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        dto.FechaModificacionHotel = now;
        return dto;
    }

    public async Task<HotelDto?> ActualizarAsync(int idHotel, HotelDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE HOTEL SET
                NOMBRE_HOTEL = @NOMBRE,
                ESTADO_HOTEL = @ESTADO,
                FECHA_MODIFICACION_HOTEL = @FECHA
            WHERE ID_HOTEL = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idHotel;
        cmd.Parameters.AddWithValue("@NOMBRE", (object?)dto.NombreHotel ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoHotel ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0) return null;

        dto.IdHotel = idHotel;
        dto.FechaModificacionHotel = now;
        return dto;
    }

    public async Task<bool> EliminarAsync(int idHotel)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
UPDATE HOTEL
SET ESTADO_HOTEL = 0
WHERE ID_HOTEL = @ID
", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idHotel;

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        return rows > 0;
    }

    private static HotelDto MapHotel(SqlDataReader dr) => new()
    {
        IdHotel = dr.GetInt32(dr.GetOrdinal("ID_HOTEL")),
        NombreHotel = dr.IsDBNull(dr.GetOrdinal("NOMBRE_HOTEL")) ? null : dr.GetString(dr.GetOrdinal("NOMBRE_HOTEL")),
        EstadoHotel = dr.IsDBNull(dr.GetOrdinal("ESTADO_HOTEL")) ? null : dr.GetBoolean(dr.GetOrdinal("ESTADO_HOTEL")),
        FechaModificacionHotel = dr.IsDBNull(dr.GetOrdinal("FECHA_MODIFICACION_HOTEL")) ? null : dr.GetDateTime(dr.GetOrdinal("FECHA_MODIFICACION_HOTEL"))
    };
}
