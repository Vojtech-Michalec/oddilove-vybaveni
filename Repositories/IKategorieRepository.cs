using OddiloveVybaveni.Models;

namespace OddiloveVybaveni.Repositories;

public interface IKategorieRepository
{
    Task<IEnumerable<KategorieVybaveni>> GetAllAsync();
}
