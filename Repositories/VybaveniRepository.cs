using Npgsql;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public class VybaveniRepository : IVybaveniRepository
{
    private readonly string _connStr;

    public VybaveniRepository(DatabaseConfig config) => _connStr = config.ConnectionString;

    private static Vybaveni MapRow(NpgsqlDataReader r) => new()
    {
        Id = r.GetInt32(0),
        Nazev = r.GetString(1),
        Popis = r.IsDBNull(2) ? null : r.GetString(2),
        PocetKusu = r.GetInt32(3),
        KategorieId = r.GetInt32(4),
        Kategorie = new KategorieVybaveni { Id = r.GetInt32(4), Nazev = r.GetString(5) }
    };

    public async Task<IEnumerable<Vybaveni>> GetAllAsync()
    {
        var list = new List<Vybaveni>();
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            """
            SELECT v.id, v.nazev, v.popis, v.pocet_kusu, v.kategorie_id, k.nazev
            FROM vybaveni v
            JOIN kategorie_vybaveni k ON k.id = v.kategorie_id
            ORDER BY v.nazev
            """, conn);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(MapRow(r));
        return list;
    }

    public async Task<Vybaveni?> GetByIdAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            """
            SELECT v.id, v.nazev, v.popis, v.pocet_kusu, v.kategorie_id, k.nazev
            FROM vybaveni v
            JOIN kategorie_vybaveni k ON k.id = v.kategorie_id
            WHERE v.id = @id
            """, conn);
        cmd.Parameters.AddWithValue("id", id);
        await using var r = await cmd.ExecuteReaderAsync();
        return await r.ReadAsync() ? MapRow(r) : null;
    }

    public async Task<Vybaveni> CreateAsync(Vybaveni v)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            """
            INSERT INTO vybaveni (nazev, popis, pocet_kusu, kategorie_id)
            VALUES (@nazev, @popis, @pocet, @kat)
            RETURNING id
            """, conn);
        cmd.Parameters.AddWithValue("nazev", v.Nazev);
        cmd.Parameters.AddWithValue("popis", (object?)v.Popis ?? DBNull.Value);
        cmd.Parameters.AddWithValue("pocet", v.PocetKusu);
        cmd.Parameters.AddWithValue("kat", v.KategorieId);
        v.Id = (int)(await cmd.ExecuteScalarAsync())!;
        return v;
    }

    public async Task UpdateAsync(Vybaveni v)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            """
            UPDATE vybaveni
            SET nazev = @nazev, popis = @popis, pocet_kusu = @pocet, kategorie_id = @kat
            WHERE id = @id
            """, conn);
        cmd.Parameters.AddWithValue("nazev", v.Nazev);
        cmd.Parameters.AddWithValue("popis", (object?)v.Popis ?? DBNull.Value);
        cmd.Parameters.AddWithValue("pocet", v.PocetKusu);
        cmd.Parameters.AddWithValue("kat", v.KategorieId);
        cmd.Parameters.AddWithValue("id", v.Id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("DELETE FROM vybaveni WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
