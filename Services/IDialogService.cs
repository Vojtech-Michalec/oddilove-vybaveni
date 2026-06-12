// ============================================================
// INTERFACE: DIALOG SERVICE
// Definuje co aplikace umí zobrazit jako modální okno.
// ViewModely volají tento interface — neví nic o konkrétních oknech.
// To je důležité pro MVVM: ViewModel nesmí přímo vytvářet UI.
// ============================================================

using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Services;

public interface IDialogService
{
    // Otevře formulář pro přidání/úpravu vybavení
    // existing = null → přidáváme nové, existing != null → upravujeme existující
    // Vrátí uložený objekt nebo null pokud uživatel zrušil
    Task<Vybaveni?> ShowVybaveniFormAsync(Vybaveni? existing = null);

    // Otevře formulář pro přidání/úpravu výpůjčky
    // vybaveniId = ID vybavení ke kterému výpůjčka patří
    Task<Vypujcka?> ShowVypujckaFormAsync(int vybaveniId, Vypujcka? existing = null);

    // Zobrazí modální okno s chybovou zprávou (uživatel klikne OK)
    Task ShowErrorAsync(string message);

    // Zobrazí potvrzovací dialog "Opravdu smazat X?"
    // Vrátí true pokud uživatel potvrdil, false pokud zrušil
    Task<bool> ConfirmDeleteAsync(string nazev);
}
