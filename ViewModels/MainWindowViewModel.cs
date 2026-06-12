// ============================================================
// VIEWMODEL: HLAVNÍ OKNO (navigace)
// Řídí co se zobrazuje v hlavním okně.
// Je to "router" celé aplikace — přepíná mezi stránkami.
// ============================================================

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    // CurrentPage = aktuálně zobrazená "stránka" (ViewModel)
    // [ObservableProperty] automaticky generuje:
    //   - public property CurrentPage { get; set; }
    //   - volání OnPropertyChanged("CurrentPage") při změně
    // Avalonia MainWindow má ContentControl napojený na CurrentPage.
    // Díky DataTemplates v App.axaml Avalonia ví: VybaveniListViewModel → zobraz VybaveniListView
    [ObservableProperty]
    private object _currentPage = null!;

    // Konstruktor — zobrazí hned seznam jako první stránku
    public MainWindowViewModel()
    {
        NavigateToList();
    }

    // ----------------------------------------------------------------
    // NAVIGACE NA SEZNAM VYBAVENÍ
    // ----------------------------------------------------------------
    public void NavigateToList()
    {
        // Získá nový VybaveniListViewModel z DI kontejneru (Transient = čistá instance)
        var vm = AppServices.Provider.GetRequiredService<VybaveniListViewModel>();

        // Předá callback — ViewModel nesmí znát MainWindowViewModel přímo
        // Jen dostane funkci "co zavolat když chce přejít na detail"
        vm.NavigateToDetail = NavigateToDetail;

        // Přepne stránku → Avalonia automaticky zobrazí VybaveniListView
        CurrentPage = vm;

        // Spustí async načítání dat z DB (fire-and-forget)
        // _ = záměrně ignorujeme Task (nevyčkáváme zde)
        _ = vm.LoadAsync();
    }

    // ----------------------------------------------------------------
    // NAVIGACE NA DETAIL VYBAVENÍ
    // ----------------------------------------------------------------
    public void NavigateToDetail(Vybaveni vybaveni)
    {
        var vm = AppServices.Provider.GetRequiredService<VybaveniDetailViewModel>();

        // Předá callback pro tlačítko "Zpět"
        vm.NavigateBack = NavigateToList;

        CurrentPage = vm;

        // Načte detail konkrétního vybavení podle jeho ID
        _ = vm.LoadAsync(vybaveni.Id);
    }
}
