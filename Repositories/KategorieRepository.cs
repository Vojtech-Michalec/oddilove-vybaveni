// ============================================================
// IMPLEMENTACE: REPOSITORY PRO KATEGORIE
// Jednoduchý SELECT — kategorie se nemění za běhu aplikace
// ============================================================

using Npgsql;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public class KategorieRepository : IKategorieRepository
{
    // Connection string uložený jako privátní pole
    private readonly string _connStr;

    // Konstruktor — DI kontejner automaticky předá DatabaseConfig
    public KategorieRepository(DatabaseConfig config) => _connStr = config.ConnectionString;

    public async Task<IEnumerable<KategorieVybaveni>> GetAllAsync()
    {
        var list = new List<KategorieVybaveni>();

        // await using = asynchronní using — spojení se zavře po dokončení bloku
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync(); // otevře spojení s PostgreSQL

        await using var cmd = new NpgsqlCommand(
            "SELECT id, nazev FROM kategorie_vybaveni ORDER BY nazev", conn);

        await using var reader = await cmd.ExecuteReaderAsync(); // spustí SELECT

        // Čte řádky jeden po druhém dokud nejsou další
        while (await reader.ReadAsync())
            list.Add(new KategorieVybaveni
            {
                Id = reader.GetInt32(0),   // sloupec 0 = id
                Nazev = reader.GetString(1) // sloupec 1 = nazev
            });

        return list;
    }
}
