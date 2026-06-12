using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public interface IVybaveniRepository
{
    Task<IEnumerable<Vybaveni>> GetAllAsync();
    Task<Vybaveni?> GetByIdAsync(int id);
    Task<Vybaveni> CreateAsync(Vybaveni vybaveni);
    Task UpdateAsync(Vybaveni vybaveni);
    Task DeleteAsync(int id);
}
