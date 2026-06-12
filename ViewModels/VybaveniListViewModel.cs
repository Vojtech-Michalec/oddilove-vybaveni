using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OddiloveVybaveni.Models;
using OddiloveVybaveni.Repositories;
using OddiloveVybaveni.Services;

namespace OddiloveVybaveni.ViewModels;

public partial class VybaveniListViewModel : ObservableObject
{
    private readonly IVybaveniRepository _repo;
    private readonly IDialogService _dialogs;

    public ObservableCollection<Vybaveni> Items { get; } = new();
    public Action<Vybaveni>? NavigateToDetail { get; set; }

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _searchText = string.Empty;

    private List<Vybaveni> _allItems = new();

    public VybaveniListViewModel(IVybaveniRepository repo, IDialogService dialogs)
    {
        _repo = repo;
        _dialogs = dialogs;
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    public async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            _allItems = (await _repo.GetAllAsync()).ToList();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            await _dialogs.ShowErrorAsync($"Chyba při načítání: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    private void ApplyFilter()
    {
        var query = SearchText.Trim().ToLower();
        var filtered = string.IsNullOrEmpty(query)
            ? _allItems
            : _allItems.Where(v =>
                v.Nazev.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (v.Kategorie?.Nazev.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
        Items.Clear();
        foreach (var item in filtered) Items.Add(item);
    }

    [RelayCommand]
    private void OpenDetail(Vybaveni vybaveni) => NavigateToDetail?.Invoke(vybaveni);

    [RelayCommand]
    private async Task AddAsync()
    {
        var result = await _dialogs.ShowVybaveniFormAsync();
        if (result is not null) await LoadAsync();
    }

    [RelayCommand]
    private async Task EditAsync(Vybaveni vybaveni)
    {
        var result = await _dialogs.ShowVybaveniFormAsync(vybaveni);
        if (result is not null) await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(Vybaveni vybaveni)
    {
        if (!await _dialogs.ConfirmDeleteAsync(vybaveni.Nazev)) return;
        try
        {
            await _repo.DeleteAsync(vybaveni.Id);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            await _dialogs.ShowErrorAsync($"Nelze smazat: {ex.Message}");
        }
    }
}
