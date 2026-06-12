// ============================================================
// VIEWMODEL: FORMULÁŘ PRO VYBAVENÍ (View 3)
// Jeden ViewModel pro oba případy: přidání i úpravu.
// Validuje vstupy a vrátí výsledek přes event CloseRequested.
// ============================================================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.ViewModels;

public partial class VybaveniFormViewModel : ObservableObject
{
    // ID existujícího záznamu (0 = nový, >0 = editace)
    private readonly int _existingId;

    // Seznam kategorií pro ComboBox — načítá DialogService před otevřením okna
    public List<KategorieVybaveni> Kategorie { get; }

    // Pomocné vlastnosti pro dynamický titulek okna
    public bool IsEdit => _existingId > 0;
    public string Title => IsEdit ? "Upravit vybavení" : "Přidat vybavení";

    // Event pro zavření okna — ViewModel nemůže sám zavřít okno (narušilo by MVVM)
    // DialogService naslouchá: vm.CloseRequested += result => win.Close(result)
    public event Action<Vybaveni?>? CloseRequested;

    // ----------------------------------------------------------------
    // POLE FORMULÁŘE — každé odpovídá jednomu TextBoxu/ComboBoxu ve View
    // [NotifyPropertyChangedFor] = při změně hodnoty notifikuje také chybovou property
    // Tím se chybová hláška automaticky aktualizuje při psaní
    // ----------------------------------------------------------------

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NazevError))]
    private string _nazev = string.Empty;

    [ObservableProperty]
    private string _popis = string.Empty;

    // PocetKusu je string (ne int) — TextBox pracuje se stringy, validujeme ručně
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PocetError))]
    private string _pocetKusu = "1";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(KategorieError))]
    private KategorieVybaveni? _selectedKategorie;

    [ObservableProperty]
    private string _globalError = string.Empty;

    // ----------------------------------------------------------------
    // VALIDAČNÍ PROPERTIES — vrátí chybový text nebo prázdný string
    // Zobrazují se jen pokud _submitted == true (uživatel klikl Uložit)
    // ----------------------------------------------------------------

    // Chyba názvu — zobrazí se pokud je prázdný A uživatel se pokusil uložit
    public string NazevError => string.IsNullOrWhiteSpace(Nazev) && _submitted
        ? "Název je povinný." : string.Empty;

    // Chyba počtu — musí být celé kladné číslo
    public string PocetError
    {
        get
        {
            if (!_submitted) return string.Empty; // před prvním pokusem nevalidujeme
            // int.TryParse = bezpečný převod, nevyhodí výjimku při špatném vstupu
            if (!int.TryParse(PocetKusu, out var n) || n < 1)
                return "Počet kusů musí být kladné číslo.";
            return string.Empty;
        }
    }

    // Chyba kategorie — musí být vybrána
    public string KategorieError => SelectedKategorie is null && _submitted
        ? "Vyberte kategorii." : string.Empty;

    // Flag — validace se zobrazí až po prvním kliknutí "Uložit"
    private bool _submitted;

    // ----------------------------------------------------------------
    // KONSTRUKTOR
    // kategorie = seznam pro ComboBox, existing = null pro nový záznam
    // ----------------------------------------------------------------
    public VybaveniFormViewModel(List<KategorieVybaveni> kategorie, Vybaveni? existing = null)
    {
        Kategorie = kategorie;
        if (existing is not null)
        {
            // Předvyplň formulář daty existujícího záznamu
            _existingId = existing.Id;
            _nazev = existing.Nazev;
            _popis = existing.Popis ?? string.Empty;
            _pocetKusu = existing.PocetKusu.ToString();
            // Najde odpovídající kategorii v seznamu podle ID
            _selectedKategorie = kategorie.FirstOrDefault(k => k.Id == existing.KategorieId);
        }
    }

    // ----------------------------------------------------------------
    // COMMAND: ULOŽIT
    // ----------------------------------------------------------------
    [RelayCommand]
    private void Save()
    {
        // Aktivuj validaci — teď se začnou zobrazovat chybové hlášky
        _submitted = true;

        // Ručně notifikuj View aby překreslil chybové texty
        OnPropertyChanged(nameof(NazevError));
        OnPropertyChanged(nameof(PocetError));
        OnPropertyChanged(nameof(KategorieError));

        // Pokud jsou chyby, neuložíme — uživatel musí opravit vstupy
        if (!string.IsNullOrWhiteSpace(NazevError) ||
            !string.IsNullOrWhiteSpace(PocetError) ||
            !string.IsNullOrWhiteSpace(KategorieError)) return;

        // Sestav výsledný objekt
        var vybaveni = new Vybaveni
        {
            Id = _existingId,   // 0 = nový, >0 = edit
            Nazev = Nazev.Trim(),
            Popis = string.IsNullOrWhiteSpace(Popis) ? null : Popis.Trim(),
            PocetKusu = int.Parse(PocetKusu), // bezpečné — PocetError by zachytil neplatný vstup
            KategorieId = SelectedKategorie!.Id,
            Kategorie = SelectedKategorie
        };

        // Předej výsledek DialogService přes event — DialogService okno zavře a uloží do DB
        CloseRequested?.Invoke(vybaveni);
    }

    // ----------------------------------------------------------------
    // COMMAND: ZRUŠIT — zavře okno bez uložení
    // ----------------------------------------------------------------
    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(null); // null = zrušeno
}
