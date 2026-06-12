// ============================================================
// VIEWMODEL: FORMULÁŘ PRO VÝPŮJČKU
// Spravuje formulář pro přidání nebo úpravu výpůjčky.
// Zajišťuje konverzi mezi DateOnly (DB) a DateTimeOffset (Avalonia DatePicker).
// ============================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.ViewModels;

public partial class VypujckaFormViewModel : ObservableObject
{
    // ID vybavení ke kterému výpůjčka patří — uloží se jako cizí klíč
    private readonly int _vybaveniId;

    // ID existující výpůjčky (0 = nová, >0 = editace)
    private readonly int _existingId;

    public bool IsEdit => _existingId > 0;
    public string Title => IsEdit ? "Upravit výpůjčku" : "Nová výpůjčka";

    // Event pro zavření okna — stejný pattern jako VybaveniFormViewModel
    public event Action<Vypujcka?>? CloseRequested;

    // ----------------------------------------------------------------
    // POLE FORMULÁŘE
    // ----------------------------------------------------------------

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(JmenoError))]
    private string _jmenoCloveka = string.Empty;

    // Avalonia DatePicker pracuje s DateTimeOffset, DB ukládá DateOnly
    // Proto používáme DateTimeOffset a konvertujeme při ukládání/načítání
    [ObservableProperty]
    private DateTimeOffset _datumVypujcky = DateTimeOffset.Now;

    // Nullable = datum vrácení je nepovinné
    [ObservableProperty]
    private DateTimeOffset? _datumVraceni;

    [ObservableProperty]
    private string _poznamka = string.Empty;

    // Checkbox "Vybavení bylo vráceno" — ovládá viditelnost pole DatumVraceni
    [ObservableProperty]
    private bool _maVraceni;

    // ----------------------------------------------------------------
    // Hook — zavolá se AUTOMATICKY při změně MaVraceni (zaškrtnutí checkboxu)
    // ----------------------------------------------------------------
    partial void OnMaVraceniChanged(bool value)
    {
        if (!value)
            DatumVraceni = null;           // odškrtl → vymaz datum vrácení
        else
            DatumVraceni ??= DateTimeOffset.Now; // zaškrtl → nastav dnešek (pokud ještě není)
        // ??= = null-coalescing assignment: přiřadí jen pokud je hodnota null
    }

    // ----------------------------------------------------------------
    // VALIDACE
    // ----------------------------------------------------------------
    public string JmenoError => string.IsNullOrWhiteSpace(JmenoCloveka) && _submitted
        ? "Jméno osoby je povinné." : string.Empty;

    private bool _submitted;

    // ----------------------------------------------------------------
    // KONSTRUKTOR
    // ----------------------------------------------------------------
    public VypujckaFormViewModel(int vybaveniId, Vypujcka? existing = null)
    {
        _vybaveniId = vybaveniId;
        if (existing is not null)
        {
            // Předvyplnění formuláře
            _existingId = existing.Id;
            _jmenoCloveka = existing.JmenoCloveka;

            // Konverze DateOnly → DateTimeOffset pro Avalonia DatePicker
            // ToDateTime(TimeOnly.MinValue) = přidá čas 00:00:00
            // TimeSpan.Zero = UTC offset 0 (bez časového pásma)
            _datumVypujcky = new DateTimeOffset(
                existing.DatumVypujcky.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

            if (existing.DatumVraceni.HasValue)
            {
                _maVraceni = true; // zaškrtne checkbox
                _datumVraceni = new DateTimeOffset(
                    existing.DatumVraceni.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            }
            _poznamka = existing.Poznamka ?? string.Empty;
        }
    }

    // ----------------------------------------------------------------
    // COMMAND: ULOŽIT
    // ----------------------------------------------------------------
    [RelayCommand]
    private void Save()
    {
        _submitted = true;
        OnPropertyChanged(nameof(JmenoError));
        if (!string.IsNullOrWhiteSpace(JmenoError)) return; // validace selhala

        var vypujcka = new Vypujcka
        {
            Id = _existingId,
            VybaveniId = _vybaveniId,
            JmenoCloveka = JmenoCloveka.Trim(),
            // Konverze DateTimeOffset → DateOnly pro uložení do DB
            // LocalDateTime = datum v lokálním časovém pásmu
            DatumVypujcky = DateOnly.FromDateTime(DatumVypujcky.LocalDateTime),
            // Datum vrácení jen pokud je zaškrtnuto a vyplněno
            DatumVraceni = MaVraceni && DatumVraceni.HasValue
                ? DateOnly.FromDateTime(DatumVraceni.Value.LocalDateTime)
                : null,
            Poznamka = string.IsNullOrWhiteSpace(Poznamka) ? null : Poznamka.Trim()
        };

        CloseRequested?.Invoke(vypujcka);
    }

    // ----------------------------------------------------------------
    // COMMAND: ZRUŠIT
    // ----------------------------------------------------------------
    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(null);
}
