using CoworkingGes.DTO;
using CoworkingGes.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoworkingGes.Repositories
{
    public class AbonnementRepository : IAbonnementRepository
    {
        private readonly Context _context;

        public AbonnementRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Abonnement>> GetAllAsync()
        {
            return await _context.abonnement
                .Include(a => a.Utilisateur)
                .Include(a=> a.Espace)
                .ToListAsync();
        }

        public async Task<Abonnement?> GetByIdAsync(int id)
        {
            return await _context.abonnement
                .Include(a => a.Utilisateur)
                .Include(a => a.Espace)
                .FirstOrDefaultAsync(a => a.Id == id);  
        }

        public async Task<IEnumerable<Abonnement>> GetByUserIdAsync(int userId)
        {
            return await _context.abonnement
                .Where(a => a.UtilisateurId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(AbonnementDTO abonnement)
        {
            Abonnement abonnement1 = new Abonnement()
            {
                Type = abonnement.Type,
                Prix = abonnement.Prix,
                DateDebut= abonnement.DateDebut,
                DateFin= abonnement.DateFin,
                Statut=abonnement.Statut,
                UtilisateurId = abonnement.UtilisateurId,
                EspaceId= abonnement.EspaceId
            };

            await _context.abonnement.AddAsync(abonnement1);
        }

        public async Task UpdateAsync(Abonnement abonnement)
        {
            _context.abonnement.Update(abonnement);
        }

        public async Task DeleteAsync(Abonnement abonnement)
        {
            _context.abonnement.Remove(abonnement);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Abonnement>> GetByEspacesIdAsync(int userId)
        {
            return await _context.abonnement
                .Where(a => a.EspaceId == userId)
                .ToListAsync();
        }
    }
}
