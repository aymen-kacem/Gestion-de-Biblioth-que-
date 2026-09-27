using CoworkingGes.DTO;
using CoworkingGes.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoworkingGes.Repositories
{
    public class RessourceRepository : IRessourceRepository
    {
        private readonly Context _context;

        public RessourceRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ressource>> GetAllAsync()
        {
            return await _context.ressource
                .Include(r => r.Espace)
                .Include(r=>r.Utilisateur)
                .ToListAsync();
        }

        public async Task<Ressource?> GetByIdAsync(int id)
        {
            return await _context.ressource
                .Include(r => r.Espace)
                .Include(r => r.Utilisateur)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Ressource>> GetByEspaceIdAsync(int espaceId)
        {
            return await _context.ressource
                .Where(r => r.EspaceId == espaceId)
                .ToListAsync();
        }

        public async Task AddAsync(RessourceDTO ressource)
        {
            Ressource reso = new Ressource()
            {
                Nom = ressource.Nom,
                Type = ressource.Type,
                QuantiteDisponible = ressource.QuantiteDisponible,
                EspaceId = ressource.EspaceId,
                UtilisateurId = ressource.UtilisateurId

            };
            await _context.ressource.AddAsync(reso);
        }

        public async Task UpdateAsync(Ressource ressource)
        {
            _context.ressource.Update(ressource);

        }

        public async Task DeleteAsync(Ressource ressource)
        {
            _context.ressource.Remove(ressource);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
