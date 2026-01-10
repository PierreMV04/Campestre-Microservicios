using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class HabxResRepository
{
    private readonly string _connectionString;

    public HabxResRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<HabxResDto>> ObtenerTodosAsync()
    {
        var lista = new List<HabxResDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HABXRES, ID_HABITACION, ID_RESERVA,
                   CAPACIDAD_RESERVA_HABXRES, COSTO_CALCULADO_HABXRES,
                   DESCUENTO_HABXRES, IMPUESTOS_HABXRES,
                   ESTADO_HABXRES, FECHA_MODIFICACION_HABXRES
            FROM HABXRES", cn);

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }

    public async Task<HabxResDto?> ObtenerPorIdAsync(int idHabxRes)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            SELECT ID_HABXRES, ID_HABITACION, ID_RESERVA,
                   CAPACIDAD_RESERVA_HABXRES, COSTO_CALCULADO_HABXRES,
                   DESCUENTO_HABXRES, IMPUESTOS_HABXRES,
                   ESTADO_HABXRES, FECHA_MODIFICACION_HABXRES
            FROM HABXRES
            WHERE ID_HABXRES = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idHabxRes;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<HabxResDto> CrearAsync(HabxResDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            INSERT INTO HABXRES
            (ID_HABITACION, ID_RESERVA, CAPACIDAD_RESERVA_HABXRES,
             COSTO_CALCULADO_HABXRES, DESCUENTO_HABXRES, IMPUESTOS_HABXRES,
             ESTADO_HABXRES, FECHA_MODIFICACION_HABXRES)
            VALUES (@HAB, @RES, @CAP, @COSTO, @DESC, @IMP, @ESTADO, @FECHA)", cn);

        cmd.Parameters.Add("@HAB", SqlDbType.Char, 10).Value = dto.IdHabitacion;
        cmd.Parameters.Add("@RES", SqlDbType.Int).Value = dto.IdReserva;
        cmd.Parameters.Add("@CAP", SqlDbType.Int).Value = (object?)dto.CapacidadReservaHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@COSTO", SqlDbType.Decimal).Value = (object?)dto.CostoCalculadoHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@DESC", SqlDbType.Decimal).Value = (object?)dto.DescuentoHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@IMP", SqlDbType.Decimal).Value = (object?)dto.ImpuestosHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value = dto.EstadoHabxRes ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        dto.FechaModificacionHabxRes = now;
        return dto;
    }

    public async Task<HabxResDto?> ActualizarAsync(int idHabxRes, HabxResDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
            UPDATE HABXRES SET
                CAPACIDAD_RESERVA_HABXRES = @CAP,
                COSTO_CALCULADO_HABXRES = @COSTO,
                DESCUENTO_HABXRES = @DESC,
                IMPUESTOS_HABXRES = @IMP,
                ESTADO_HABXRES = @ESTADO,
                FECHA_MODIFICACION_HABXRES = @FECHA
            WHERE ID_HABXRES = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idHabxRes;
        cmd.Parameters.Add("@CAP", SqlDbType.Int).Value = (object?)dto.CapacidadReservaHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@COSTO", SqlDbType.Decimal).Value = (object?)dto.CostoCalculadoHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@DESC", SqlDbType.Decimal).Value = (object?)dto.DescuentoHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@IMP", SqlDbType.Decimal).Value = (object?)dto.ImpuestosHabxRes ?? DBNull.Value;
        cmd.Parameters.Add("@ESTADO", SqlDbType.Bit).Value = dto.EstadoHabxRes ?? true;
        cmd.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = now;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(int idHabxRes)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(
            @"
UPDATE HABXRES
SET ESTADO_HABXRES = 0
WHERE ID_HABXRES = @ID", cn);

        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idHabxRes;

        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
    public async Task<List<HabxResDto>> ObtenerPorReservaAsync(int idReserva)
    {
        var lista = new List<HabxResDto>();

        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
        SELECT ID_HABXRES, ID_HABITACION, ID_RESERVA,
               CAPACIDAD_RESERVA_HABXRES, COSTO_CALCULADO_HABXRES,
               DESCUENTO_HABXRES, IMPUESTOS_HABXRES,
               ESTADO_HABXRES, FECHA_MODIFICACION_HABXRES
        FROM HABXRES
        WHERE ID_RESERVA = @ID_RESERVA", cn);

        cmd.Parameters.Add("@ID_RESERVA", SqlDbType.Int).Value = idReserva;

        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
            lista.Add(Map(dr));

        return lista;
    }


    private static HabxResDto Map(SqlDataReader dr) => new()
    {
        IdHabxRes = dr.GetInt32(0),
        IdHabitacion = dr.GetString(1).Trim(),
        IdReserva = dr.GetInt32(2),
        CapacidadReservaHabxRes = dr.IsDBNull(3) ? null : dr.GetInt32(3),
        CostoCalculadoHabxRes = dr.IsDBNull(4) ? null : dr.GetDecimal(4),
        DescuentoHabxRes = dr.IsDBNull(5) ? null : dr.GetDecimal(5),
        ImpuestosHabxRes = dr.IsDBNull(6) ? null : dr.GetDecimal(6),
        EstadoHabxRes = dr.IsDBNull(7) ? null : dr.GetBoolean(7),
        FechaModificacionHabxRes = dr.IsDBNull(8) ? null : dr.GetDateTime(8)
    };
}
