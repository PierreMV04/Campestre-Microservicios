using Microsoft.Data.SqlClient;
using Shared.DTOs;
using System.Data;

namespace Shared.Data;

public class PdfRepository
{
    private readonly string _connectionString;

    public PdfRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
    }

    public async Task<List<PdfDto>> ObtenerTodosAsync()
    {
        var lista = new List<PdfDto>();
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_PDF, ID_FACTURA, URL_PDF, ESTADO_PDF, FECHA_MODIFICACION_PDF FROM PDF", cn);
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync()) lista.Add(Map(dr));
        return lista;
    }

    public async Task<PdfDto?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_PDF, ID_FACTURA, URL_PDF, ESTADO_PDF, FECHA_MODIFICACION_PDF FROM PDF WHERE ID_PDF = @ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<PdfDto?> ObtenerPorFacturaAsync(int idFactura)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SELECT ID_PDF, ID_FACTURA, URL_PDF, ESTADO_PDF, FECHA_MODIFICACION_PDF FROM PDF WHERE ID_FACTURA = @ID_FAC", cn);
        cmd.Parameters.Add("@ID_FAC", SqlDbType.Int).Value = idFactura;
        await cn.OpenAsync();
        await using var dr = await cmd.ExecuteReaderAsync();
        return await dr.ReadAsync() ? Map(dr) : null;
    }

    public async Task<PdfDto> CrearAsync(PdfDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"INSERT INTO PDF (ID_PDF, ID_FACTURA, URL_PDF, ESTADO_PDF, FECHA_MODIFICACION_PDF) 
            VALUES (@ID, @ID_FAC, @URL, @ESTADO, @FECHA)", cn);
        cmd.Parameters.AddWithValue("@ID", dto.IdPdf);
        cmd.Parameters.AddWithValue("@ID_FAC", dto.IdFactura);
        cmd.Parameters.AddWithValue("@URL", (object?)dto.UrlPdf ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoPdf ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        dto.FechaModificacionPdf = now;
        return dto;
    }

    public async Task<PdfDto?> ActualizarAsync(int id, PdfDto dto)
    {
        var now = DateTime.Now;
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"UPDATE PDF SET ID_FACTURA=@ID_FAC, URL_PDF=@URL, ESTADO_PDF=@ESTADO, FECHA_MODIFICACION_PDF=@FECHA 
            WHERE ID_PDF=@ID", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        cmd.Parameters.AddWithValue("@ID_FAC", dto.IdFactura);
        cmd.Parameters.AddWithValue("@URL", (object?)dto.UrlPdf ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ESTADO", dto.EstadoPdf ?? true);
        cmd.Parameters.AddWithValue("@FECHA", now);
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0 ? dto : null;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        await using var cn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(@"
UPDATE PDF
SET ESTADO_PDF = 0
WHERE ID_PDF = @ID
", cn);
        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
        await cn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static PdfDto Map(SqlDataReader dr) => new()
    {
        IdPdf = dr.GetInt32(0),
        IdFactura = dr.GetInt32(1),
        UrlPdf = dr.IsDBNull(2) ? null : dr.GetString(2),
        EstadoPdf = dr.IsDBNull(3) ? null : dr.GetBoolean(3),
        FechaModificacionPdf = dr.IsDBNull(4) ? null : dr.GetDateTime(4)
    };
}
