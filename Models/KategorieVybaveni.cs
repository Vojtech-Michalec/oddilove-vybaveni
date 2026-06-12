// ============================================================
// MODEL: KATEGORIE VYBAVENÍ (číselník)
// Odpovídá tabulce kategorie_vybaveni v databázi.
// Číselník = pevný seznam hodnot, spravovaný přes seed.sql
// ============================================================

namespace OddiloveVybaveni.Models;

public class KategorieVybaveni
{
    // Primární klíč — databáze ho generuje automaticky (SERIAL)
    public int Id { get; set; }

    // Název kategorie, např. "Stan / přístřeší", "Lano / lezení"
    public string Nazev { get; set; } = string.Empty;

    // Přepíše výchozí ToString() — Avalonia ComboBox zobrazuje ToString() jako text položky
    // Bez toho by ComboBox zobrazoval "OddiloveVybaveni.Models.KategorieVybaveni"
    public override string ToString() => Nazev;
}
