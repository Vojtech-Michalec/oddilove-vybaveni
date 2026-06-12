using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.ViewModels;

public partial class VybaveniFormViewModel : ObservableObject
{
    private readonly int _existingId;

    public List<KategorieVybaveni> Kategorie { get; }
    public bool IsEdit => _existingId > 0;
    public string Title => IsEdit ? "Upravit vybavení" : "Přidat vybavení";
    public event Action<Vybaveni?>? CloseRequested;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NazevError))]
    private string _nazev = string.Empty;

    [ObservableProperty]
    private string _popis = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PocetError))]
    private string _pocetKusu = "1";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(KategorieError))]
    private KategorieVybaveni? _selectedKategorie;

    [ObservableProperty]
    private string _globalError = string.Empty;

    public string NazevError => string.IsNullOrWhiteSpace(Nazev) && _submitted
        ? "Název je povinný." : string.Empty;

    public string PocetError
    {
        get
        {
            if (!_submitted) return string.Empty;
            if (!int.TryParse(PocetKusu, out var n) || n < 1)
                return "Počet kusů musí být kladné číslo.";
            return string.Empty;
        }
    }

    public string KategorieError => SelectedKategorie is null && _submitted
        ? "Vyberte kategorii." : string.Empty;

    private bool _submitted;

    public VybaveniFormViewModel(List<KategorieVybaveni> kategorie, Vybaveni? existing = null)
    {
        Kategorie = kategorie;
        if (existing is not null)
        {
            _existingId = existing.Id;
            _nazev = existing.Nazev;
            _popis = existing.Popis ?? string.Empty;
            _pocetKusu = existing.PocetKusu.ToString();
            _selectedKategorie = kategorie.FirstOrDefault(k => k.Id == existing.KategorieId);
        }
    }

    [RelayCommand]
    private void Save()
    {
        _submitted = true;
        OnPropertyChanged(nameof(NazevError));
        OnPropertyChanged(nameof(PocetError));
        OnPropertyChanged(nameof(KategorieError));

        if (!string.IsNullOrWhiteSpace(NazevError) ||
            !string.IsNullOrWhiteSpace(PocetError) ||
            !string.IsNullOrWhiteSpace(KategorieError)) return;

        var vybaveni = new Vybaveni
        {
            Id = _existingId,
            Nazev = Nazev.Trim(),
            Popis = string.IsNullOrWhiteSpace(Popis) ? null : Popis.Trim(),
            PocetKusu = int.Parse(PocetKusu),
            KategorieId = SelectedKategorie!.Id,
            Kategorie = SelectedKategorie
        };
        CloseRequested?.Invoke(vybaveni);
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(null);
}
