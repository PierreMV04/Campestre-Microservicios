using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class AmexHabRepository
{
    private readonly string _connectionString;

    public AmexHabRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<AmexHabDto>> ObtenerTodosAsync()
    {
        var lista = new List<AmexHabDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HABITACION, ID_AMENIDAD, ESTADO_AMEXHAB, FECHA_MODIFICACION_AMEXHAB
            FROM AMEXHAB", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<AmexHabDto?> ObtenerPorIdAsync(string idHabitacion, int idAmenidad)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HABITACION, ID_AMENIDAD, ESTADO_AMEXHAB, FECHA_MODIFICACION_AMEXHAB
            FROM AMEXHAB
            WHERE ID_HABITACION = @ID_HAB AND ID_AMENIDAD = @ID_AME", cn);

        cmd.Parameters.Add("@ID_HAB", SqlDbType.Char, 10).Value = idHabitacion;
        cmd.Parameters.Add("@ID_AME", SqlDbType.Int).Value = idAmenidad;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<List<AmexHabDto>> ObtenerPorHabitacionAsync(string idHabitacion)
    {
        var lista = new List<AmexHabDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HABITACION, ID_AMENIDAD, ESTADO_AMEXHAB, FECHA_MODIFICACION_AMEXHAB
            FROM AMEXHAB
            WHERE ID_HABITACION = @ID_HAB", cn);

        cmd.Parameters.Add("@ID_HAB", SqlDbType.Char, 10).Value = idHabitacion;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<AmexHabDto> CrearAsync(AmexHabDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO AMEXHAB
            (ID_HABITACION, ID_AMENIDAD, ESTADO_AMEXHAB, FECHA_MODIFICACION_AMEXHAB)
            VALUES (@ID_HAB, @ID_AME, @ESTADO, @FECHA)", cn);

        cmd.Parameters.Add("@ID_HAB", SqlDbType.Char, 10).Value = dto.IdHabitacion;
        cmd.Parameters.Add("@ID_AME", SqlDbType.Int).Value = dto.IdAmenidad;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value = dto.EstadoAmexHab ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        dto.FechaModificacionAmexHab = now;
        return dto;
    }

    public async Task<AmexHabDto?> ActualizarAsync(string idHabitacion, int idAmenidad, AmexHabDto dto)
    {
        var now = DateTime.Now;

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE AMEXHAB SET
                ESTADO_AMEXHAB = @ESTADO,
                FECHA_MODIFICACION_AMEXHAB = @FECHA
            WHERE ID_HABITACION = @ID_HAB AND ID_AMENIDAD = @ID_AME", cn);

        cmd.Parameters.Add("@ID_HAB", SqlDbType.Char, 10).Value = idHabitacion;
        cmd.Parameters.Add("@ID_AME", SqlDbType.Int).Value = idAmenidad;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value = dto.EstadoAmexHab ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0) return null;

        dto.FechaModificacionAmexHab = now;
        return dto;
    }

    public async Task<bool> EliminarAsync(string idHabitacion, int idAmenidad)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
UPDATE AMEXHAB
SET ESTADO_AMEXHAB = 0
WHERE ID_AMENIDAD = @ID_AME
AND ID_HABITACION = @ID_HAB", cn);

        cmd.Parameters.Add("@ID_HAB", SqlDbType.Char, 10).Value = idHabitacion;
        cmd.Parameters.Add("@ID_AME", SqlDbType.Int).Value = idAmenidad;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static AmexHabDto Map(SqlDataReader dr) => new()
    {
        IdHabitacion = dr.GetString(0).Trim(), // char(10) → string
        IdAmenidad = dr.GetInt32(1),
        EstadoAmexHab = dr.IsDBNull(2) ? null : dr.GetBoolean(2),
        FechaModificacionAmexHab = dr.IsDBNull(3) ? null : dr.GetDateTime(3)
    };
}
