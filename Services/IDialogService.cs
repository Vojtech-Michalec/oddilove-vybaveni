using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Services;

public interface IDialogService
{
    Task<Vybaveni?> ShowVybaveniFormAsync(Vybaveni? existing = null);
    Task<Vypujcka?> ShowVypujckaFormAsync(int vybaveniId, Vypujcka? existing = null);
    Task ShowErrorAsync(string message);
    Task<bool> ConfirmDeleteAsync(string nazev);
}
