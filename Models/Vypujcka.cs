// ============================================================
// MODEL: VÝPŮJČKA (dětská entita)
// Odpovídá tabulce vypujcka v databázi.
// Každá výpůjčka patří právě jednomu vybavení (N:1)
// ============================================================

namespace OddiloveVybaveni.Models;

public class Vypujcka
{
    // Primární klíč — generuje databáze
    public int Id { get; set; }

    // Cizí klíč na tabulku vybaveni — propojuje výpůjčku s vybavením
    public int VybaveniId { get; set; }

    // Jméno osoby, která si vybavení půjčila
    public string JmenoCloveka { get; set; } = string.Empty;

    // DateOnly = pouze datum bez času (.NET 6+), mapuje se na PostgreSQL typ DATE
    // Výchozí hodnota = dnešní datum
    public DateOnly DatumVypujcky { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    // Datum vrácení — null znamená "ještě nevráceno"
    // ? = nullable typ
    public DateOnly? DatumVraceni { get; set; }

    // Volitelná poznámka k výpůjčce
    public string? Poznamka { get; set; }

    // Computed property — vypočítá se z dat, neukládá se do databáze
    // HasValue = true pokud datum vrácení bylo zadáno
    public bool JeVraceno => DatumVraceni.HasValue;
}
