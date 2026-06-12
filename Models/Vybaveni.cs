// ============================================================
// MODEL: VYBAVENÍ (hlavní entita)
// Odpovídá tabulce vybaveni v databázi.
// Jedna položka vybavení může mít více výpůjček (1:N vztah)
// ============================================================

namespace OddiloveVybaveni.Models;

public class Vybaveni
{
    // Primární klíč — generuje databáze
    public int Id { get; set; }

    // Název vybavení, např. "Velký stan Coleman"
    public string Nazev { get; set; } = string.Empty;

    // Volitelný popis — může být null (proto ?)
    public string? Popis { get; set; }

    // Kolik kusů máme celkem k dispozici (výchozí 1)
    public int PocetKusu { get; set; } = 1;

    // Cizí klíč do tabulky kategorie_vybaveni — ukládá se do databáze
    public int KategorieId { get; set; }

    // Navigační vlastnost — objekt kategorie načtený přes JOIN
    // Není sloupec v DB, jen ho naplníme v repository při čtení
    // ? = může být null pokud byl objekt vytvořen bez JOIN dotazu
    public KategorieVybaveni? Kategorie { get; set; }
}
