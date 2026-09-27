using CoworkingGes.DTO;
using CoworkingGes.Models;

namespace CoworkingGes.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);
        Task<int> GetAvailableCapacityAsync(int espaceId, DateTime debut, DateTime fin);
        Task<bool> IsAvailableAsync(int espaceId, DateTime debut, DateTime fin);
        Task AddAsync(ReservationDTO reservation);
        Task UpdateAsync(Reservation reservation);
        Task DeleteAsync(Reservation reservation);
        Task SaveChangesAsync();
    }
}