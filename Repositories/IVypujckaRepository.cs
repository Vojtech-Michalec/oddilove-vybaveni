// ============================================================
// INTERFACE: REPOSITORY PRO VÝPŮJČKY
// Výpůjčky jsou dětská entita — vždy patří konkrétnímu vybavení.
// Proto zde není GetAll() — výpůjčky čteme vždy pro konkrétní vybavení.
// ============================================================

using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public interface IVypujckaRepository
{
    // Načte všechny výpůjčky pro dané vybavení (podle vybaveni_id)
    Task<IEnumerable<Vypujcka>> GetByVybaveniIdAsync(int vybaveniId);

    // Vytvoří novou výpůjčku, vrátí objekt s přiděleným ID
    Task<Vypujcka> CreateAsync(Vypujcka vypujcka);

    // Aktualizuje existující výpůjčku (datum vrácení, poznámka...)
    Task UpdateAsync(Vypujcka vypujcka);

    // Smaže výpůjčku podle ID
    Task DeleteAsync(int id);
}
