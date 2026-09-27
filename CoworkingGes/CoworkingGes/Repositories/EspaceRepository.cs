using CoworkingGes.DTO;
using CoworkingGes.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoworkingGes.Repositories
{
    public class EspaceRepository : IEspaceRepository
    {
        private readonly Context _context;

        public EspaceRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Espace>> GetAllAsync()
        {
            return await _context.espace.Include(e => e.Ressources).Include(e=>e.Utilisateur).ToListAsync();
        }

        public async Task<Espace?> GetByIdAsync(int id)
        {
            return await _context.espace
                .Include(e => e.Ressources)           // ✅ Add this
                .Include(e => e.Reservations)         // ✅ Add this
                .Include(e => e.Maintenances)         // ✅ Add this
                .Include(e => e.Abonnements)          // ✅ Add this
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task AddAsync(EspaceDTO espace)
        {
            Espace espace1 = new Espace()
            {
                Nom = espace.Nom,
                Type = espace.Type,
                Capacite = espace.Capacite,
                Localisation = espace.Localisation,
                UtilisateurId = espace.UtilisateurId

            };
            await _context.espace.AddAsync(espace1);
        }

        public async Task UpdateAsync(Espace espace)
        {
            _context.espace.Update(espace);
        }

        public async Task DeleteAsync(Espace espace)
        {

            _context.espace.Remove(espace);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
