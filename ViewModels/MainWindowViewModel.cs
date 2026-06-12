using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object _currentPage = null!;

    public MainWindowViewModel()
    {
        NavigateToList();
    }

    public void NavigateToList()
    {
        var vm = AppServices.Provider.GetRequiredService<VybaveniListViewModel>();
        vm.NavigateToDetail = NavigateToDetail;
        CurrentPage = vm;
        _ = vm.LoadAsync();
    }

    public void NavigateToDetail(Vybaveni vybaveni)
    {
        var vm = AppServices.Provider.GetRequiredService<VybaveniDetailViewModel>();
        vm.NavigateBack = NavigateToList;
        CurrentPage = vm;
        _ = vm.LoadAsync(vybaveni.Id);
    }
}
