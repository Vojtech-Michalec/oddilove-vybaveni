namespace OddiloveVybaveni.Models;

public class Vybaveni
{
    public int Id { get; set; }
    public string Nazev { get; set; } = string.Empty;
    public string? Popis { get; set; }
    public int PocetKusu { get; set; } = 1;
    public int KategorieId { get; set; }
    public KategorieVybaveni? Kategorie { get; set; }
}
