namespace OddiloveVybaveni.Models;

public class Vypujcka
{
    public int Id { get; set; }
    public int VybaveniId { get; set; }
    public string JmenoCloveka { get; set; } = string.Empty;
    public DateOnly DatumVypujcky { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? DatumVraceni { get; set; }
    public string? Poznamka { get; set; }

    public bool JeVraceno => DatumVraceni.HasValue;
}
