using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public interface IVypujckaRepository
{
    Task<IEnumerable<Vypujcka>> GetByVybaveniIdAsync(int vybaveniId);
    Task<Vypujcka> CreateAsync(Vypujcka vypujcka);
    Task UpdateAsync(Vypujcka vypujcka);
    Task DeleteAsync(int id);
}
