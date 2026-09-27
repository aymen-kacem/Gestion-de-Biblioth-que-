using CoworkingGes.DTO;
using CoworkingGes.Enum;
using CoworkingGes.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoworkingGes.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly Context _context;

        public ReservationRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.reservation
                .Include(r => r.Espace)
                .Include(r => r.Utilisateur)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.reservation
                .Include(r => r.Espace)
                .Include(r => r.Utilisateur)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
        {
            return await _context.reservation
                .Where(r => r.UtilisateurId == userId)
                .Include(r => r.Espace)
                .ToListAsync();
        }

        public async Task<int> GetAvailableCapacityAsync(int espaceId, DateTime debut, DateTime fin)
        {
            // Get the espace and its total capacity
            var espace = await _context.espace.FirstOrDefaultAsync(e => e.Id == espaceId);
            if (espace == null)
                throw new ArgumentException("Espace not found");

            int totalCapacity = espace.Capacite;

            // Count confirmed reservations that overlap with the requested time range
            var reservationCount = await _context.reservation
                .CountAsync(r => r.EspaceId == espaceId &&
                                 r.Statut == Statut.Confirmé &&
                                 r.DateDebut < fin &&
                                 r.DateFin > debut);

            // Count confirmed abonnements that are active during the requested time range
            var abonnementCount = await _context.abonnement
                .CountAsync(a => a.Statut == Statut.Confirmé &&
                                 a.DateDebut <= fin &&
                                 a.DateFin >= debut);

            // Calculate available capacity
            int usedCapacity = reservationCount + abonnementCount;
            int availableCapacity = totalCapacity - usedCapacity;

            return Math.Max(0, availableCapacity);
        }

        public async Task<bool> IsAvailableAsync(int espaceId, DateTime debut, DateTime fin)
        {
            int availableCapacity = await GetAvailableCapacityAsync(espaceId, debut, fin);
            return availableCapacity > 0;
        }

        public async Task AddAsync(ReservationDTO reservation)
        {
            Reservation reservation1 = new Reservation()
            {
                EspaceId = reservation.EspaceId,
                UtilisateurId = reservation.UtilisateurId,
                DateDebut = reservation.DateDebut,
                DateFin = reservation.DateFin,
                Statut = reservation.Statut,
                Notes = reservation.Notes ?? ""
            };
            await _context.reservation.AddAsync(reservation1);
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.reservation.Update(reservation);
        }

        public async Task DeleteAsync(Reservation reservation)
        {
            _context.reservation.Remove(reservation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}