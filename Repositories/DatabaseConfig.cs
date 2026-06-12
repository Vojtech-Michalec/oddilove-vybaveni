// ============================================================
// KONFIGURACE DATABÁZE
// Jednoduchá wrapper třída pro connection string.
// Registrována jako Singleton v DI — všechny repositories ji sdílejí.
// ============================================================

namespace OddiloveVybaveni.Repositories;

public class DatabaseConfig
{
    // Connection string pro připojení k PostgreSQL
    // Formát: "Host=localhost;Port=5432;Username=oddil;Password=xxx;Database=oddil_vybaveni"
    public string ConnectionString { get; }

    // Konstruktor přijme connection string a uloží ho
    public DatabaseConfig(string connectionString) => ConnectionString = connectionString;
}
