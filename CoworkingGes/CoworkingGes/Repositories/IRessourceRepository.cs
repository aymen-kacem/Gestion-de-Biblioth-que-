using CoworkingGes.DTO;
using CoworkingGes.Models;

namespace CoworkingGes.Repositories
{
    public interface IRessourceRepository
    {
        Task<IEnumerable<Ressource>> GetAllAsync();
        Task<Ressource?> GetByIdAsync(int id);
        Task<IEnumerable<Ressource>> GetByEspaceIdAsync(int espaceId);
        Task AddAsync(RessourceDTO ressource);
        Task UpdateAsync(Ressource ressource);
        Task DeleteAsync(Ressource ressource);
        Task SaveChangesAsync();
    }
}
