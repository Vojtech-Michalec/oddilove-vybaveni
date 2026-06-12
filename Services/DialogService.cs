using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using OddiloveVybaveni.Models;
using OddiloveVybaveni.Repositories;
using OddiloveVybaveni.ViewModels;
using OddiloveVybaveni.Views;

namespace OddiloveVybaveni.Services;

public class DialogService : IDialogService
{
    private readonly IKategorieRepository _kategorieRepo;
    private readonly IVybaveniRepository _vybaveniRepo;
    private readonly IVypujckaRepository _vypujckaRepo;

    public DialogService(
        IKategorieRepository kategorieRepo,
        IVybaveniRepository vybaveniRepo,
        IVypujckaRepository vypujckaRepo)
    {
        _kategorieRepo = kategorieRepo;
        _vybaveniRepo = vybaveniRepo;
        _vypujckaRepo = vypujckaRepo;
    }

    private static Window GetMainWindow() =>
        ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!)
        .MainWindow!;

    public async Task<Vybaveni?> ShowVybaveniFormAsync(Vybaveni? existing = null)
    {
        var kategorie = (await _kategorieRepo.GetAllAsync()).ToList();
        var vm = new VybaveniFormViewModel(kategorie, existing);
        var win = new VybaveniFormWindow { DataContext = vm };
        vm.CloseRequested += result => win.Close(result);
        var entity = await win.ShowDialog<Vybaveni?>(GetMainWindow());
        if (entity is null) return null;

        if (entity.Id == 0)
            return await _vybaveniRepo.CreateAsync(entity);

        await _vybaveniRepo.UpdateAsync(entity);
        return entity;
    }

    public async Task<Vypujcka?> ShowVypujckaFormAsync(int vybaveniId, Vypujcka? existing = null)
    {
        var vm = new VypujckaFormViewModel(vybaveniId, existing);
        var win = new VypujckaFormWindow { DataContext = vm };
        vm.CloseRequested += result => win.Close(result);
        var entity = await win.ShowDialog<Vypujcka?>(GetMainWindow());
        if (entity is null) return null;

        if (entity.Id == 0)
            return await _vypujckaRepo.CreateAsync(entity);

        await _vypujckaRepo.UpdateAsync(entity);
        return entity;
    }

    public async Task ShowErrorAsync(string message)
    {
        var win = new ErrorWindow(message);
        await win.ShowDialog(GetMainWindow());
    }

    public async Task<bool> ConfirmDeleteAsync(string nazev)
    {
        var win = new ConfirmWindow($"Opravdu smazat \"{nazev}\"?");
        return await win.ShowDialog<bool>(GetMainWindow());
    }
}
