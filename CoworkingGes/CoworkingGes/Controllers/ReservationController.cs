using CoworkingGes.DTO;
using CoworkingGes.Enum;
using CoworkingGes.Models;
using CoworkingGes.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoworkingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IAbonnementRepository _abonnementRepository;

        public ReservationController(
            IReservationRepository reservationRepository,
            IAbonnementRepository abonnementRepository)
        {
            _reservationRepository = reservationRepository;
            _abonnementRepository = abonnementRepository;
        }


        // =================== ADMIN ONLY ===================

        // GET: api/Reservation - Admin only (get all reservations)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            return Ok(reservations);
        }

        // GET: api/Reservation/{id} - Admin only
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Reservation not found" });

            return Ok(reservation);
        }

        // GET: api/Reservation/user/{userId} - Admin only
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var reservations = await _reservationRepository.GetByUserIdAsync(userId);
            return Ok(reservations);
        }

        // GET: api/Reservation/espace/{espaceId}/available-capacity - Check available capacity
        [HttpGet("espace/{espaceId}/available-capacity")]
        [Authorize(Roles = "Etudiant,Admin")]
        public async Task<IActionResult> GetAvailableCapacity(int espaceId, [FromQuery] DateTime debut, [FromQuery] DateTime fin)
        {
            try
            {
                int availableCapacity = await _reservationRepository.GetAvailableCapacityAsync(espaceId, debut, fin);
                return Ok(new { espaceId, availableCapacity, debut, fin });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // =================== ETUDIANT + ADMIN ===================

        // GET: api/Reservation/my-reservations - Etudiant and Admin can view own
        [HttpGet("my-reservations")]
        [Authorize(Roles = "Etudiant,Admin")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var reservations = await _reservationRepository.GetByUserIdAsync(userId);
            return Ok(reservations);
        }

        [HttpPost]
        [Authorize(Roles = "Etudiant")]
        public async Task<IActionResult> Create(ReservationDTO reservation)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            // Basic validation
            if (reservation.DateDebut >= reservation.DateFin)
                return BadRequest(new { message = "DateDebut must be before DateFin." });

            if (reservation.DateDebut < DateTime.UtcNow.Date)
                return BadRequest(new { message = "Cannot create reservation in the past." });

            // Assign user id
            if (userRole == "Etudiant")
                reservation.UtilisateurId = currentUserId;

            // =================🔥 OVERLAP CHECK =================
            // Check against existing reservations
            var userReservations = await _reservationRepository.GetByUserIdAsync(currentUserId);
            bool reservationOverlaps = userReservations.Any(r =>
                reservation.DateDebut < r.DateFin && reservation.DateFin > r.DateDebut
            );

            if (reservationOverlaps)
            {
                return BadRequest(new
                {
                    message = "You already have a reservation that overlaps with this date range."
                });
            }

            // Check against existing abonnements
            var userAbonnements = await _abonnementRepository.GetByUserIdAsync(currentUserId);
            bool abonnementOverlaps = userAbonnements.Any(a =>
                reservation.DateDebut < a.DateFin && reservation.DateFin > a.DateDebut
            );

            if (abonnementOverlaps)
            {
                return BadRequest(new
                {
                    message = "You have an active abonnement that overlaps with this date range."
                });
            }
            // =====================================================

            // Check espace capacity
            try
            {
                int availableCapacity = await _reservationRepository.GetAvailableCapacityAsync(
                    reservation.EspaceId,
                    reservation.DateDebut,
                    reservation.DateFin
                );

                if (availableCapacity <= 0)
                {
                    reservation.Statut = Statut.Rejetée;
                    reservation.Notes = "Reservation rejected: No available capacity.";
                }
                else
                {
                    reservation.Statut = Statut.Confirmé;
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveChangesAsync();

            return Ok(reservation);
        }




        // PUT: api/Reservation/{id} - Etudiant updates their own, Admin updates any
        [HttpPut("{id}")]
        [Authorize(Roles = "Etudiant,Admin")]
        public async Task<IActionResult> Update(int id, Reservation updated)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var existing = await _reservationRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Reservation not found" });

            // If Etudiant: can only update their own reservations
            if (userRole == "Etudiant" && existing.UtilisateurId != currentUserId)
                return Forbid();

            // If trying to update a confirmed reservation with new dates, re-validate capacity
            if (existing.Statut == Statut.Confirmé &&
                (updated.DateDebut != existing.DateDebut || updated.DateFin != existing.DateFin))
            {
                try
                {
                    int availableCapacity = await _reservationRepository.GetAvailableCapacityAsync(
                        existing.EspaceId,
                        updated.DateDebut,
                        updated.DateFin
                    );

                    // Add back the current reservation to the count since we're checking availability for the new dates
                    availableCapacity += 1;

                    if (availableCapacity <= 0)
                        return BadRequest(new { message = "No available capacity for the new time range." });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            existing.DateDebut = updated.DateDebut;
            existing.DateFin = updated.DateFin;
            existing.Statut = updated.Statut;
            existing.Notes = updated.Notes;

            await _reservationRepository.UpdateAsync(existing);
            await _reservationRepository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Reservation/{id} - Etudiant deletes their own, Admin deletes any
        [HttpDelete("{id}")]
        [Authorize(Roles = "Etudiant,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Reservation not found" });

            // If Etudiant: can only delete their own reservations
            if (userRole == "Etudiant" && reservation.UtilisateurId != currentUserId)
                return Forbid();

            await _reservationRepository.DeleteAsync(reservation);
            await _reservationRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}