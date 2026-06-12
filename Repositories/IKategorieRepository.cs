// ============================================================
// INTERFACE: REPOSITORY PRO KATEGORIE (číselník)
// Kategorie jsou jen pro čtení — spravují se přes seed.sql
// Proto jen GetAll(), žádné Create/Update/Delete
// ============================================================

using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public interface IKategorieRepository
{
    // Načte všechny kategorie seřazené podle názvu
    // Používá se při otevření formuláře pro naplnění ComboBoxu
    Task<IEnumerable<KategorieVybaveni>> GetAllAsync();
}
