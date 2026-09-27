using CoworkingGes.DTO;
using CoworkingGes.Models;

namespace CoworkingGes.Repositories
{
    public interface IAbonnementRepository
    {
        Task<IEnumerable<Abonnement>> GetAllAsync();//admin
        Task<Abonnement?> GetByIdAsync(int id);//admin
        Task<IEnumerable<Abonnement>> GetByUserIdAsync(int userId);//admin
        Task<IEnumerable<Abonnement>> GetByEspacesIdAsync(int userId);//admin
        Task AddAsync(AbonnementDTO abonnement);// etudiant 
        Task UpdateAsync(Abonnement abonnement);// etudiant admin 
        Task DeleteAsync(Abonnement abonnement);// etudiant admin
        Task SaveChangesAsync();
    }
}
