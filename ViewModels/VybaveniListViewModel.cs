// ============================================================
// VIEWMODEL: SEZNAM VYBAVENÍ (View 1)
// Logika pro obrazovku se seznamem vybavení.
// Zodpovídá za: načtení dat, filtrování, CRUD operace.
// ============================================================

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OddiloveVybaveni.Models;
using OddiloveVybaveni.Repositories;
using OddiloveVybaveni.Services;

namespace OddiloveVybaveni.ViewModels;

public partial class VybaveniListViewModel : ObservableObject
{
    // Závislosti injektované přes DI
    private readonly IVybaveniRepository _repo;    // přístup k tabulce vybaveni
    private readonly IDialogService _dialogs;       // otevírání formulářů a potvrzovacích dialogů

    // ObservableCollection = kolekce která automaticky notifikuje View při přidání/odebrání
    // View (ListBox) je na tuto kolekci navázán přes ItemsSource="{Binding Items}"
    public ObservableCollection<Vybaveni> Items { get; } = new();

    // Callback pro navigaci na detail — nastaví MainWindowViewModel při inicializaci
    public Action<Vybaveni>? NavigateToDetail { get; set; }

    // IsBusy = true pokud právě načítáme data (mohlo by zobrazit spinner)
    [ObservableProperty]
    private bool _isBusy;

    // Text z vyhledávacího pole — při změně se automaticky volá OnSearchTextChanged
    [ObservableProperty]
    private string _searchText = string.Empty;

    // Kompletní seznam načtený z DB — filtrujeme ho lokálně bez dalších DB dotazů
    private List<Vybaveni> _allItems = new();

    // Konstruktor — DI kontejner automaticky předá repository a dialog service
    public VybaveniListViewModel(IVybaveniRepository repo, IDialogService dialogs)
    {
        _repo = repo;
        _dialogs = dialogs;
    }

    // ----------------------------------------------------------------
    // Hook — zavolá se AUTOMATICKY pokaždé když se změní SearchText
    // partial void = vygeneruje CommunityToolkit.Mvvm při buildu
    // ----------------------------------------------------------------
    partial void OnSearchTextChanged(string value) => ApplyFilter();

    // ----------------------------------------------------------------
    // NAČTENÍ DAT Z DATABÁZE
    // ----------------------------------------------------------------
    public async Task LoadAsync()
    {
        IsBusy = true; // signalizuje že probíhá načítání
        try
        {
            // Načte vše z DB a uloží do interního seznamu
            _allItems = (await _repo.GetAllAsync()).ToList();
            // Aplikuje aktuální filtr (hledaný text)
            ApplyFilter();
        }
        catch (Exception ex)
        {
            // Jakákoliv chyba (DB nedostupná, špatný SQL...) → zobraz dialog
            await _dialogs.ShowErrorAsync($"Chyba při načítání: {ex.Message}");
        }
        finally
        {
            // finally se provede vždy — i pokud nastala chyba
            IsBusy = false;
        }
    }

    // ----------------------------------------------------------------
    // FILTROVÁNÍ — pracuje lokálně nad _allItems (bez volání DB)
    // ----------------------------------------------------------------
    private void ApplyFilter()
    {
        var query = SearchText.Trim().ToLower();

        // Pokud je hledaný text prázdný → zobraz vše
        var filtered = string.IsNullOrEmpty(query)
            ? _allItems
            : _allItems.Where(v =>
                // Hledej v názvu vybavení (case-insensitive)
                v.Nazev.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                // Nebo v názvu kategorie (?. = null-safe, ?? false = fallback pokud null)
                (v.Kategorie?.Nazev.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));

        // Aktualizuj ObservableCollection — View se automaticky překreslí
        Items.Clear();
        foreach (var item in filtered) Items.Add(item);
    }

    // ----------------------------------------------------------------
    // COMMAND: přejdi na detail
    // [RelayCommand] generuje property OpenDetailCommand : ICommand
    // View: Command="{Binding OpenDetailCommand}" CommandParameter="{Binding}"
    // ----------------------------------------------------------------
    [RelayCommand]
    private void OpenDetail(Vybaveni vybaveni) => NavigateToDetail?.Invoke(vybaveni);

    // ----------------------------------------------------------------
    // COMMAND: přidej nové vybavení
    // ----------------------------------------------------------------
    [RelayCommand]
    private async Task AddAsync()
    {
        // Otevře prázdný formulář, počká na výsledek
        var result = await _dialogs.ShowVybaveniFormAsync();
        // Pokud uživatel uložil (result != null) → obnov seznam
        if (result is not null) await LoadAsync();
    }

    // ----------------------------------------------------------------
    // COMMAND: uprav existující vybavení
    // vybaveni = položka ze seznamu na které uživatel klikl "Upravit"
    // ----------------------------------------------------------------
    [RelayCommand]
    private async Task EditAsync(Vybaveni vybaveni)
    {
        // Otevře předvyplněný formulář s daty vybavení
        var result = await _dialogs.ShowVybaveniFormAsync(vybaveni);
        if (result is not null) await LoadAsync();
    }

    // ----------------------------------------------------------------
    // COMMAND: smaž vybavení
    // ----------------------------------------------------------------
    [RelayCommand]
    private async Task DeleteAsync(Vybaveni vybaveni)
    {
        // Nejdřív se zeptej uživatele — pokud zrušil, nic nedělej
        if (!await _dialogs.ConfirmDeleteAsync(vybaveni.Nazev)) return;
        try
        {
            await _repo.DeleteAsync(vybaveni.Id);
            await LoadAsync(); // obnov seznam po smazání
        }
        catch (Exception ex)
        {
            // Chyba může nastat např. pokud DB není dostupná
            await _dialogs.ShowErrorAsync($"Nelze smazat: {ex.Message}");
        }
    }
}
