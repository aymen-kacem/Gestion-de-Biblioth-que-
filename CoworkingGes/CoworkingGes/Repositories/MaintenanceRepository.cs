using CoworkingGes.DTO;
using CoworkingGes.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoworkingGes.Repositories
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly Context _context;

        public MaintenanceRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Maintenance>> GetAllAsync()
        {
            return await _context.maintenance
                .Include(m => m.Espace)
                .Include(m => m.Technicien)
                .ToListAsync();
        }

        public async Task<Maintenance?> GetByIdAsync(int id)
        {
            return await _context.maintenance
                .Include(m => m.Espace)
                .Include(m => m.Technicien)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Maintenance>> GetByEspaceIdAsync(int espaceId)
        {
            return await _context.maintenance
                .Where(m => m.EspaceId == espaceId)
                .ToListAsync();
        }

        public async Task AddAsync(MaintenanceDTO maintenance)
        {
            Maintenance maintenance1 = new Maintenance()
            {
                EspaceId = maintenance.EspaceId,
                Description = maintenance.Description,
                Statut = maintenance.Statut,
                Date = maintenance.Date,
                TechnicienId =maintenance.TechnicienId

            };
            await _context.maintenance.AddAsync(maintenance1);
        }

        public async Task UpdateAsync(Maintenance maintenance)
        {
            _context.maintenance.Update(maintenance);
        }

        public async Task DeleteAsync(Maintenance maintenance)
        {
            _context.maintenance.Remove(maintenance);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        // Add this implementation to your MaintenanceRepository class
        public async Task<ICollection<Maintenance>> GetByTechnicienIdAsync(int technicienId)
        {
            return await _context.maintenance
                .Where(m => m.TechnicienId == technicienId)
                .Include(m => m.Espace)
                .Include(m => m.Technicien)
                .ToListAsync();
        }
    }
}
