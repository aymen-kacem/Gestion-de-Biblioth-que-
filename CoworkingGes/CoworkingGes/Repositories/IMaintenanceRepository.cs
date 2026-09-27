using CoworkingGes.DTO;
using CoworkingGes.Models;

namespace CoworkingGes.Repositories
{
    public interface IMaintenanceRepository
    {
        Task<IEnumerable<Maintenance>> GetAllAsync();//admin
        Task<Maintenance?> GetByIdAsync(int id);//admin
        Task<IEnumerable<Maintenance>> GetByEspaceIdAsync(int espaceId);//admin
        Task AddAsync(MaintenanceDTO maintenance);//technicien admin
        Task UpdateAsync(Maintenance maintenance);// technicien admin
        Task DeleteAsync(Maintenance maintenance);// technicien admin
        Task<ICollection<Maintenance>> GetByTechnicienIdAsync(int technicienId);
        Task SaveChangesAsync();
    }
}
