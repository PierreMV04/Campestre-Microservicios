using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class HoldRepository
{
    private readonly string _connectionString;

    public HoldRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<HoldDto>> ObtenerTodosAsync()
    {
        var lista = new List<HoldDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HOLD, ID_HABITACION, ID_RESERVA, TIEMPO_HOLD,
                   FECHA_INICIO_HOLD, FECHA_FINAL_HOLD, ESTADO_HOLD
            FROM HOLD", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<HoldDto?> ObtenerPorIdAsync(string idHold)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HOLD, ID_HABITACION, ID_RESERVA, TIEMPO_HOLD,
                   FECHA_INICIO_HOLD, FECHA_FINAL_HOLD, ESTADO_HOLD
            FROM HOLD
            WHERE ID_HOLD = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Char, 10).Value = idHold;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<List<HoldDto>> ObtenerPorHabitacionAsync(string idHabitacion)
    {
        var lista = new List<HoldDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HOLD, ID_HABITACION, ID_RESERVA, TIEMPO_HOLD,
                   FECHA_INICIO_HOLD, FECHA_FINAL_HOLD, ESTADO_HOLD
            FROM HOLD
            WHERE ID_HABITACION = @ID_HAB", cn);

        cmd.Parameters.Add("@ID_HAB", SqlDbType.Char, 10).Value = idHabitacion;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<HoldDto> CrearAsync(HoldDto dto)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO HOLD
            (ID_HOLD, ID_HABITACION, ID_RESERVA, TIEMPO_HOLD,
             FECHA_INICIO_HOLD, FECHA_FINAL_HOLD, ESTADO_HOLD)
            VALUES (@ID, @HAB, @RES, @TIEMPO, @INICIO, @FINAL, @ESTADO)", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Char, 10).Value = dto.IdHold;
        cmd.Parameters.Add("@HAB", SqlDbType.Char, 10).Value = dto.IdHabitacion;
        cmd.Parameters.Add("@RES", SqlDbType.Int).Value = dto.IdReserva;
        cmd.Parameters.Add("@TIEMPO", SqlDbType.Int).Value = (object?)dto.TiempoHold ?? DBNull.Value;
        cmd.Parameters.Add("@INICIO", SqlDbType.DateTime).Value = (object?)dto.FechaInicioHold ?? DBNull.Value;
        cmd.Parameters.Add("@FINAL", SqlDbType.DateTime).Value = (object?)dto.FechaFinalHold ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value = dto.EstadoHold ?? true;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return dto;
    }

    public async Task<HoldDto?> ActualizarAsync(string idHold, HoldDto dto)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE HOLD SET
                ID_HABITACION = @HAB,
                ID_RESERVA = @RES,
                TIEMPO_HOLD = @TIEMPO,
                FECHA_INICIO_HOLD = @INICIO,
                FECHA_FINAL_HOLD = @FINAL,
                ESTADO_HOLD = @ESTADO
            WHERE ID_HOLD = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Char, 10).Value = idHold;
        cmd.Parameters.Add("@HAB", SqlDbType.Char, 10).Value = dto.IdHabitacion;
        cmd.Parameters.Add("@RES", SqlDbType.Int).Value = dto.IdReserva;
        cmd.Parameters.Add("@TIEMPO", SqlDbType.Int).Value = (object?)dto.TiempoHold ?? DBNull.Value;
        cmd.Parameters.Add("@INICIO", SqlDbType.DateTime).Value = (object?)dto.FechaInicioHold ?? DBNull.Value;
        cmd.Parameters.Add("@FINAL", SqlDbType.DateTime).Value = (object?)dto.FechaFinalHold ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value = dto.EstadoHold ?? true;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(string idHold)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
UPDATE HOLD
SET ESTADO_HOLD = 0
WHERE ID_HOLD = @ID
", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Char, 10).Value = idHold;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static HoldDto Map(SqlDataReader dr) => new()
    {
        IdHold = dr.GetString(0).Trim(),
        IdHabitacion = dr.GetString(1).Trim(),
        IdReserva = dr.GetInt32(2),
        TiempoHold = dr.IsDBNull(3) ? null : dr.GetInt32(3),
        FechaInicioHold = dr.IsDBNull(4) ? null : dr.GetDateTime(4),
        FechaFinalHold = dr.IsDBNull(5) ? null : dr.GetDateTime(5),
        EstadoHold = dr.IsDBNull(6) ? null : dr.GetBoolean(6)
    };
}
