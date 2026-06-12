using Npgsql;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public class KategorieRepository : IKategorieRepository
{
    private readonly string _connStr;

    public KategorieRepository(DatabaseConfig config) => _connStr = config.ConnectionString;

    public async Task<IEnumerable<KategorieVybaveni>> GetAllAsync()
    {
        var list = new List<KategorieVybaveni>();
        await using var conn = new NpgsqlConnection(_connStr);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT id, nazev FROM kategorie_vybaveni ORDER BY nazev", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(new KategorieVybaveni { Id = reader.GetInt32(0), Nazev = reader.GetString(1) });
        return list;
    }
}
