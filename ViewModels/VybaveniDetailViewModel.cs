// ============================================================
// VIEWMODEL: DETAIL VYBAVENÍ (View 2)
// Zobrazuje jedno vybavení a seznam jeho výpůjček.
// Nad výpůjčkami je plný CRUD (Create, Read, Update, Delete).
// ============================================================

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OddiloveVybaveni.Models;
using OddiloveVybaveni.Repositories;
using OddiloveVybaveni.Services;

namespace OddiloveVybaveni.ViewModels;

public partial class VybaveniDetailViewModel : ObservableObject
{
    private readonly IVybaveniRepository _vybaveniRepo;
    private readonly IVypujckaRepository _vypujckaRepo;
    private readonly IDialogService _dialogs;

    // Seznam výpůjček pro toto vybavení — View na něj napojí ListBox
    public ObservableCollection<Vypujcka> Vypujcky { get; } = new();

    // Callback pro tlačítko "Zpět" — nastaví MainWindowViewModel
    public Action? NavigateBack { get; set; }

    // Aktuálně zobrazené vybavení — View zobrazuje jeho název, kategorii, popis...
    [ObservableProperty]
    private Vybaveni? _vybaveni;

    [ObservableProperty]
    private bool _isBusy;

    // Konstruktor — DI předá obě repositories a dialog service
    public VybaveniDetailViewModel(
        IVybaveniRepository vybaveniRepo,
        IVypujckaRepository vypujckaRepo,
        IDialogService dialogs)
    {
        _vybaveniRepo = vybaveniRepo;
        _vypujckaRepo = vypujckaRepo;
        _dialogs = dialogs;
    }

    // ----------------------------------------------------------------
    // NAČTENÍ DETAILU — volá MainWindowViewModel.NavigateToDetail()
    // ----------------------------------------------------------------
    public async Task LoadAsync(int id)
    {
        IsBusy = true;
        try
        {
            // Načte hlavní entitu (vybavení)
            Vybaveni = await _vybaveniRepo.GetByIdAsync(id);
            // Načte seznam výpůjček pro toto vybavení
            await ReloadVypujckyAsync();
        }
        catch (Exception ex)
        {
            await _dialogs.ShowErrorAsync($"Chyba při načítání: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    // ----------------------------------------------------------------
    // POMOCNÁ METODA: Znovu načte výpůjčky z DB
    // Voláme po každé CRUD operaci nad výpůjčkami
    // ----------------------------------------------------------------
    private async Task ReloadVypujckyAsync()
    {
        if (Vybaveni is null) return; // ochrana pokud LoadAsync ještě neskončil
        var data = await _vypujckaRepo.GetByVybaveniIdAsync(Vybaveni.Id);
        // Musíme Clear + Add (ne = new()) — View drží referenci na tuto instanci kolekce
        Vypujcky.Clear();
        foreach (var v in data) Vypujcky.Add(v);
    }

    // ----------------------------------------------------------------
    // COMMAND: Zpět na seznam
    // ----------------------------------------------------------------
    [RelayCommand]
    private void Back() => NavigateBack?.Invoke(); // zavolá NavigateToList() v MainWindowViewModel

    // ----------------------------------------------------------------
    // COMMAND: Uprav vybavení (otevře formulář)
    // ----------------------------------------------------------------
    [RelayCommand]
    private async Task EditVybaveniAsync()
    {
        if (Vybaveni is null) return;
        var result = await _dialogs.ShowVybaveniFormAsync(Vybaveni);
        if (result is not null)
            // Znovu načteme z DB — formulář mohl změnit kategorii, název atd.
            Vybaveni = await _vybaveniRepo.GetByIdAsync(Vybaveni.Id);
    }

    // ----------------------------------------------------------------
    // COMMAND: Přidej novou výpůjčku
    // ----------------------------------------------------------------
    [RelayCommand]
    private async Task AddVypujckaAsync()
    {
        if (Vybaveni is null) return;
        // Předáme ID vybavení — formulář ho uloží jako cizí klíč
        var result = await _dialogs.ShowVypujckaFormAsync(Vybaveni.Id);
        if (result is not null) await ReloadVypujckyAsync();
    }

    // ----------------------------------------------------------------
    // COMMAND: Uprav výpůjčku (přijme CommandParameter = konkrétní Vypujcka)
    // ----------------------------------------------------------------
    [RelayCommand]
    private async Task EditVypujckaAsync(Vypujcka vypujcka)
    {
        if (Vybaveni is null) return;
        // Předáme existující výpůjčku → formulář se předvyplní
        var result = await _dialogs.ShowVypujckaFormAsync(Vybaveni.Id, vypujcka);
        if (result is not null) await ReloadVypujckyAsync();
    }

    // ----------------------------------------------------------------
    // COMMAND: Smaž výpůjčku
    // ----------------------------------------------------------------
    [RelayCommand]
    private async Task DeleteVypujckaAsync(Vypujcka vypujcka)
    {
        if (!await _dialogs.ConfirmDeleteAsync($"výpůjčku pro {vypujcka.JmenoCloveka}")) return;
        try
        {
            await _vypujckaRepo.DeleteAsync(vypujcka.Id);
            await ReloadVypujckyAsync();
        }
        catch (Exception ex)
        {
            await _dialogs.ShowErrorAsync($"Chyba při mazání: {ex.Message}");
        }
    }
}
