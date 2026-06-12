namespace OddiloveVybaveni.Models;

public class KategorieVybaveni
{
    public int Id { get; set; }
    public string Nazev { get; set; } = string.Empty;

    public override string ToString() => Nazev;
}
