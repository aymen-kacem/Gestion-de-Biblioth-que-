using CoworkingGes.DTO;
using CoworkingGes.Models;

namespace CoworkingGes.Repositories
{
    public interface IEspaceRepository
    {
        Task<IEnumerable<Espace>> GetAllAsync();
        Task<Espace?> GetByIdAsync(int id);
        Task AddAsync(EspaceDTO espace);
        Task UpdateAsync(Espace espace);
        Task DeleteAsync(Espace espace);
        Task SaveChangesAsync();
    }
}
