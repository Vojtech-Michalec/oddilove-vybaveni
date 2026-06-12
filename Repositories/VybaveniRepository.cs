// ============================================================
// IMPLEMENTACE: REPOSITORY PRO VYBAVENÍ
// Obsahuje veškerý SQL pro tabulku vybaveni.
// ViewModel nikdy nevidí SQL — komunikuje jen přes interface.
// ============================================================

using Npgsql;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public class VybaveniRepository : IVybaveniRepository
{
    private readonly string _connStr;

    // DI kontejner předá DatabaseConfig — uložíme si jen string
    public VybaveniRepository(DatabaseConfig config) => _connStr = config.ConnectionString;

    // ----------------------------------------------------------------
    // HELPER: Převede jeden řádek z databáze na C# objekt
    // Čísla (0,1,2...) = pořadí sloupců v SELECT dotazu
    // ----------------------------------------------------------------
    private static Vybaveni MapRow(NpgsqlDataReader r) => new()
    {
        Id = r.GetInt32(0),                                  // v.id
        Nazev = r.GetString(1),                              // v.nazev
        Popis = r.IsDBNull(2) ? null : r.GetString(2),      // v.popis (může být NULL)
        PocetKusu = r.GetInt32(3),                           // v.pocet_kusu
        KategorieId = r.GetInt32(4),                         // v.kategorie_id
        // Rovnou naplníme navigační vlastnost z JOINu — ušetříme druhý dotaz
        Kategorie = new KategorieVybaveni { Id = r.GetInt32(4), Nazev = r.GetString(5) }
    };

    // ----------------------------------------------------------------
    // GET ALL — načte vše, seřadí podle názvu
    // ----------------------------------------------------------------
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
        // """ = raw string literal v C# 11 — lze psát SQL přes více řádků bez \n
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(MapRow(r));
        return list;
    }

    // ----------------------------------------------------------------
    // GET BY ID — načte jedno vybavení podle primárního klíče
    // ----------------------------------------------------------------
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
        // @id = parametr — NIKDY nevkládáme hodnoty přímo do SQL stringu (SQL injection!)
        cmd.Parameters.AddWithValue("id", id);
        await using var r = await cmd.ExecuteReaderAsync();
        // Ternární operátor: pokud je řádek → mapuj, jinak vrať null
        return await r.ReadAsync() ? MapRow(r) : null;
    }

    // ----------------------------------------------------------------
    // CREATE — vloží nový záznam, vrátí objekt s novým ID
    // ----------------------------------------------------------------
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
        // RETURNING id = PostgreSQL vrátí nově vygenerované ID — nemusíme dělat druhý dotaz
        cmd.Parameters.AddWithValue("nazev", v.Nazev);
        // (object?) trik: pokud je Popis null, pošleme DBNull.Value = SQL NULL
        cmd.Parameters.AddWithValue("popis", (object?)v.Popis ?? DBNull.Value);
        cmd.Parameters.AddWithValue("pocet", v.PocetKusu);
        cmd.Parameters.AddWithValue("kat", v.KategorieId);
        // ExecuteScalarAsync = spustí SQL a vrátí první hodnotu z prvního řádku (= nové ID)
        // (int)...! = přetypujeme na int, ! říká kompilátoru "vím že to není null"
        v.Id = (int)(await cmd.ExecuteScalarAsync())!;
        return v;
    }

    // ----------------------------------------------------------------
    // UPDATE — aktualizuje existující záznam
    // ----------------------------------------------------------------
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
        // ExecuteNonQueryAsync = spustí SQL, ale žádná data nečteme zpět
        await cmd.ExecuteNonQueryAsync();
    }

    // ----------------------------------------------------------------
    // DELETE — smaže vybavení, DB automaticky smaže i výpůjčky (CASCADE)
    // ----------------------------------------------------------------
    public async Task DeleteAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "DELETE FROM vybaveni WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
