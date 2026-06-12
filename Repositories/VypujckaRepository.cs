using Npgsql;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public class VypujckaRepository : IVypujckaRepository
{
    private readonly string _connStr;

    public VypujckaRepository(DatabaseConfig config) => _connStr = config.ConnectionString;

    private static Vypujcka MapRow(NpgsqlDataReader r) => new()
    {
        Id = r.GetInt32(0),
        VybaveniId = r.GetInt32(1),
        JmenoCloveka = r.GetString(2),
        DatumVypujcky = r.GetFieldValue<DateOnly>(3),
        DatumVraceni = r.IsDBNull(4) ? null : r.GetFieldValue<DateOnly>(4),
        Poznamka = r.IsDBNull(5) ? null : r.GetString(5)
    };

    public async Task<IEnumerable<Vypujcka>> GetByVybaveniIdAsync(int vybaveniId)
    {
        var list = new List<Vypujcka>();
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            """
            SELECT id, vybaveni_id, jmeno_cloveka, datum_vypujcky, datum_vraceni, poznamka
            FROM vypujcka
            WHERE vybaveni_id = @vid
            ORDER BY datum_vypujcky DESC
            """, conn);
        cmd.Parameters.AddWithValue("vid", vybaveniId);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(MapRow(r));
        return list;
    }

    public async Task<Vypujcka> CreateAsync(Vypujcka v)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            """
            INSERT INTO vypujcka (vybaveni_id, jmeno_cloveka, datum_vypujcky, datum_vraceni, poznamka)
            VALUES (@vid, @jmeno, @dv, @dr, @poz)
            RETURNING id
            """, conn);
        cmd.Parameters.AddWithValue("vid", v.VybaveniId);
        cmd.Parameters.AddWithValue("jmeno", v.JmenoCloveka);
        cmd.Parameters.AddWithValue("dv", v.DatumVypujcky);
        cmd.Parameters.AddWithValue("dr", (object?)v.DatumVraceni ?? DBNull.Value);
        cmd.Parameters.AddWithValue("poz", (object?)v.Poznamka ?? DBNull.Value);
        v.Id = (int)(await cmd.ExecuteScalarAsync())!;
        return v;
    }

    public async Task UpdateAsync(Vypujcka v)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            """
            UPDATE vypujcka
            SET jmeno_cloveka = @jmeno, datum_vypujcky = @dv, datum_vraceni = @dr, poznamka = @poz
            WHERE id = @id
            """, conn);
        cmd.Parameters.AddWithValue("jmeno", v.JmenoCloveka);
        cmd.Parameters.AddWithValue("dv", v.DatumVypujcky);
        cmd.Parameters.AddWithValue("dr", (object?)v.DatumVraceni ?? DBNull.Value);
        cmd.Parameters.AddWithValue("poz", (object?)v.Poznamka ?? DBNull.Value);
        cmd.Parameters.AddWithValue("id", v.Id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("DELETE FROM vypujcka WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
