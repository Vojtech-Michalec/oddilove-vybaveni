// ============================================================
// INTERFACE: REPOSITORY PRO VYBAVENÍ
// Definuje CO repository umí — bez jakékoliv implementace.
// ViewModely pracují s tímto interfacem, ne s konkrétní třídou.
// Výhoda: lze snadno změnit implementaci (jiná DB, mock pro testy)
// ============================================================

using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public interface IVybaveniRepository
{
    // Načte všechna vybavení včetně jejich kategorie (JOIN)
    Task<IEnumerable<Vybaveni>> GetAllAsync();

    // Načte jedno vybavení podle ID, vrátí null pokud neexistuje
    Task<Vybaveni?> GetByIdAsync(int id);

    // Vytvoří nový záznam v DB, vrátí objekt s přiděleným ID
    Task<Vybaveni> CreateAsync(Vybaveni vybaveni);

    // Aktualizuje existující záznam podle ID
    Task UpdateAsync(Vybaveni vybaveni);

    // Smaže záznam podle ID (kaskádově smaže i výpůjčky — viz schema.sql)
    Task DeleteAsync(int id);
}
