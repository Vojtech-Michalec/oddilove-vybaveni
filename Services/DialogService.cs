// ============================================================
// IMPLEMENTACE: DIALOG SERVICE
// Zajišťuje otevírání modálních oken a ukládání výsledků do DB.
// Je to "most" mezi ViewModely a konkrétními okny Avalonii.
// ============================================================

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
    // Repositories injektované přes DI — potřebujeme je pro načtení kategorií a uložení dat
    private readonly IKategorieRepository _kategorieRepo;
    private readonly IVybaveniRepository _vybaveniRepo;
    private readonly IVypujckaRepository _vypujckaRepo;

    // Konstruktor — DI kontejner automaticky předá všechny závislosti
    public DialogService(
        IKategorieRepository kategorieRepo,
        IVybaveniRepository vybaveniRepo,
        IVypujckaRepository vypujckaRepo)
    {
        _kategorieRepo = kategorieRepo;
        _vybaveniRepo = vybaveniRepo;
        _vypujckaRepo = vypujckaRepo;
    }

    // Pomocná metoda: získá reference na hlavní okno aplikace
    // ShowDialog potřebuje "owner" okno — modální dialog se na něj "přichytí"
    private static Window GetMainWindow() =>
        ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!)
        .MainWindow!;

    // ----------------------------------------------------------------
    // FORMULÁŘ PRO VYBAVENÍ (přidání nebo úprava)
    // ----------------------------------------------------------------
    public async Task<Vybaveni?> ShowVybaveniFormAsync(Vybaveni? existing = null)
    {
        // 1. Načti kategorie z DB → naplní ComboBox ve formuláři
        var kategorie = (await _kategorieRepo.GetAllAsync()).ToList();

        // 2. Vytvoř ViewModel pro formulář
        //    existing = null → prázdný formulář, existing != null → předvyplněný
        var vm = new VybaveniFormViewModel(kategorie, existing);

        // 3. Vytvoř okno a napoj ViewModel jako DataContext
        var win = new VybaveniFormWindow { DataContext = vm };

        // 4. Napoj event — když ViewModel zavolá CloseRequested, zavři okno s výsledkem
        //    result = Vybaveni objekt nebo null (pokud uživatel zrušil)
        vm.CloseRequested += result => win.Close(result);

        // 5. Zobraz modální dialog a ČEKEJ dokud uživatel neuzavře okno
        //    ShowDialog<T> blokuje (asynchronně) a vrátí co jsme předali do Close()
        var entity = await win.ShowDialog<Vybaveni?>(GetMainWindow());

        // 6. Uživatel zrušil → nic neukládáme
        if (entity is null) return null;

        // 7. Uložení do DB: Id == 0 = nový záznam, Id > 0 = existující
        if (entity.Id == 0)
            return await _vybaveniRepo.CreateAsync(entity); // INSERT

        await _vybaveniRepo.UpdateAsync(entity); // UPDATE
        return entity;
    }

    // ----------------------------------------------------------------
    // FORMULÁŘ PRO VÝPŮJČKU (přidání nebo úprava)
    // ----------------------------------------------------------------
    public async Task<Vypujcka?> ShowVypujckaFormAsync(int vybaveniId, Vypujcka? existing = null)
    {
        // ViewModel dostane ID vybavení (musí ho uložit jako FK) a případná existující data
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

    // ----------------------------------------------------------------
    // CHYBOVÝ DIALOG
    // ----------------------------------------------------------------
    public async Task ShowErrorAsync(string message)
    {
        var win = new ErrorWindow(message);
        // ShowDialog bez generického parametru = jen čekáme na zavření, žádná návratová hodnota
        await win.ShowDialog(GetMainWindow());
    }

    // ----------------------------------------------------------------
    // POTVRZOVACÍ DIALOG (před smazáním)
    // ----------------------------------------------------------------
    public async Task<bool> ConfirmDeleteAsync(string nazev)
    {
        var win = new ConfirmWindow($"Opravdu smazat \"{nazev}\"?");
        // ShowDialog<bool> = vrátí true (smazat) nebo false (zrušit)
        return await win.ShowDialog<bool>(GetMainWindow());
    }
}
