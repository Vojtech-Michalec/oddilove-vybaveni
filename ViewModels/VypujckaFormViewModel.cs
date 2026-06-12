using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.ViewModels;

public partial class VypujckaFormViewModel : ObservableObject
{
    private readonly int _vybaveniId;
    private readonly int _existingId;

    public bool IsEdit => _existingId > 0;
    public string Title => IsEdit ? "Upravit výpůjčku" : "Nová výpůjčka";
    public event Action<Vypujcka?>? CloseRequested;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(JmenoError))]
    private string _jmenoCloveka = string.Empty;

    [ObservableProperty]
    private DateTimeOffset _datumVypujcky = DateTimeOffset.Now;

    [ObservableProperty]
    private DateTimeOffset? _datumVraceni;

    [ObservableProperty]
    private string _poznamka = string.Empty;

    [ObservableProperty]
    private bool _maVraceni;

    partial void OnMaVraceniChanged(bool value)
    {
        if (!value) DatumVraceni = null;
        else DatumVraceni ??= DateTimeOffset.Now;
    }

    public string JmenoError => string.IsNullOrWhiteSpace(JmenoCloveka) && _submitted
        ? "Jméno osoby je povinné." : string.Empty;

    private bool _submitted;

    public VypujckaFormViewModel(int vybaveniId, Vypujcka? existing = null)
    {
        _vybaveniId = vybaveniId;
        if (existing is not null)
        {
            _existingId = existing.Id;
            _jmenoCloveka = existing.JmenoCloveka;
            _datumVypujcky = new DateTimeOffset(
                existing.DatumVypujcky.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            if (existing.DatumVraceni.HasValue)
            {
                _maVraceni = true;
                _datumVraceni = new DateTimeOffset(
                    existing.DatumVraceni.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            }
            _poznamka = existing.Poznamka ?? string.Empty;
        }
    }

    [RelayCommand]
    private void Save()
    {
        _submitted = true;
        OnPropertyChanged(nameof(JmenoError));
        if (!string.IsNullOrWhiteSpace(JmenoError)) return;

        var vypujcka = new Vypujcka
        {
            Id = _existingId,
            VybaveniId = _vybaveniId,
            JmenoCloveka = JmenoCloveka.Trim(),
            DatumVypujcky = DateOnly.FromDateTime(DatumVypujcky.LocalDateTime),
            DatumVraceni = MaVraceni && DatumVraceni.HasValue
                ? DateOnly.FromDateTime(DatumVraceni.Value.LocalDateTime)
                : null,
            Poznamka = string.IsNullOrWhiteSpace(Poznamka) ? null : Poznamka.Trim()
        };
        CloseRequested?.Invoke(vypujcka);
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(null);
}
