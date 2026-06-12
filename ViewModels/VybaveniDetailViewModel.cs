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

    public ObservableCollection<Vypujcka> Vypujcky { get; } = new();
    public Action? NavigateBack { get; set; }

    [ObservableProperty]
    private Vybaveni? _vybaveni;

    [ObservableProperty]
    private bool _isBusy;

    public VybaveniDetailViewModel(
        IVybaveniRepository vybaveniRepo,
        IVypujckaRepository vypujckaRepo,
        IDialogService dialogs)
    {
        _vybaveniRepo = vybaveniRepo;
        _vypujckaRepo = vypujckaRepo;
        _dialogs = dialogs;
    }

    public async Task LoadAsync(int id)
    {
        IsBusy = true;
        try
        {
            Vybaveni = await _vybaveniRepo.GetByIdAsync(id);
            await ReloadVypujckyAsync();
        }
        catch (Exception ex)
        {
            await _dialogs.ShowErrorAsync($"Chyba při načítání: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    private async Task ReloadVypujckyAsync()
    {
        if (Vybaveni is null) return;
        var data = await _vypujckaRepo.GetByVybaveniIdAsync(Vybaveni.Id);
        Vypujcky.Clear();
        foreach (var v in data) Vypujcky.Add(v);
    }

    [RelayCommand]
    private void Back() => NavigateBack?.Invoke();

    [RelayCommand]
    private async Task EditVybaveniAsync()
    {
        if (Vybaveni is null) return;
        var result = await _dialogs.ShowVybaveniFormAsync(Vybaveni);
        if (result is not null)
            Vybaveni = await _vybaveniRepo.GetByIdAsync(Vybaveni.Id);
    }

    [RelayCommand]
    private async Task AddVypujckaAsync()
    {
        if (Vybaveni is null) return;
        var result = await _dialogs.ShowVypujckaFormAsync(Vybaveni.Id);
        if (result is not null) await ReloadVypujckyAsync();
    }

    [RelayCommand]
    private async Task EditVypujckaAsync(Vypujcka vypujcka)
    {
        if (Vybaveni is null) return;
        var result = await _dialogs.ShowVypujckaFormAsync(Vybaveni.Id, vypujcka);
        if (result is not null) await ReloadVypujckyAsync();
    }

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
