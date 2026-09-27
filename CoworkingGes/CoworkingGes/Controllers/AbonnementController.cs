using CoworkingGes.DTO;
using CoworkingGes.Enum;
using CoworkingGes.Helpers;
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
    public class AbonnementController : ControllerBase
    {
        private readonly IAbonnementRepository _abonnementRepository;
        private readonly IReservationRepository _reservationRepository;

        public AbonnementController(IAbonnementRepository abonnementRepository, IReservationRepository reservationRepository)
        {
            _abonnementRepository = abonnementRepository;
            _reservationRepository = reservationRepository;
        }

        // =================== ADMIN ONLY ===================

        // GET: api/Abonnement - Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var abonnements = await _abonnementRepository.GetAllAsync();
            return Ok(abonnements);
        }

        // GET: api/Abonnement/{id} - Admin only
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var abonnement = await _abonnementRepository.GetByIdAsync(id);
            if (abonnement == null)
                return NotFound(new { message = "Abonnement not found" });

            return Ok(abonnement);
        }

        // GET: api/Abonnement/user/{userId} - Admin only
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var abonnements = await _abonnementRepository.GetByUserIdAsync(userId);
            return Ok(abonnements);
        }

        // GET: api/Abonnement/espace/{espaceId} - Admin only
        [HttpGet("espace/{espaceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByEspace(int espaceId)
        {
            var abonnements = await _abonnementRepository.GetByEspacesIdAsync(espaceId);
            return Ok(abonnements);
        }

        // =================== ETUDIANT + ADMIN ===================

        // GET: api/Abonnement/my-abonnements - Etudiant can view their own
        [HttpGet("my-abonnements")]
        [Authorize(Roles = "Etudiant,Admin")]
        public async Task<IActionResult> GetMyAbonnements()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var abonnements = await _abonnementRepository.GetByUserIdAsync(userId);
            return Ok(abonnements);
        }

        // =================== CREATE WITH RULES ADDED ===================
        [HttpPost]
        [Authorize(Roles = "Etudiant,Admin")]
        public async Task<IActionResult> Create(AbonnementDTO abonnement)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userIdClaim == null) return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            if (!AbonnementTypeHelper.AbonnementTypes.ContainsKey(abonnement.Type))
                return BadRequest(new { message = "Invalid abonnement type" });

            if (abonnement.EspaceId == 0)
                return BadRequest(new { message = "EspaceId is required" });

            if (userRole == "Etudiant")
                abonnement.UtilisateurId = currentUserId;

            // 🔥 Auto Price
            abonnement.Prix = AbonnementTypeHelper.GetPrice(abonnement.Type);

            // 🔥 Auto DateFin based on type
            abonnement.DateFin = abonnement.Type switch
            {
                "Mensuel" => abonnement.DateDebut.AddMonths(1),
                "Trimestrel" => abonnement.DateDebut.AddMonths(3),
                "Semestriel" => abonnement.DateDebut.AddMonths(6),
                "Annuel" => abonnement.DateDebut.AddYears(1),
                _ => abonnement.DateDebut
            };

            // 🔥 Check student cannot overlap previous abonnement
            var lastAbonnement = (await _abonnementRepository.GetByUserIdAsync(abonnement.UtilisateurId))
                                .OrderByDescending(a => a.DateFin)
                                .FirstOrDefault();

            if (lastAbonnement != null && abonnement.DateDebut <= lastAbonnement.DateFin)
            {
                return BadRequest(new
                {
                    message = $"You already have an abonnement until {lastAbonnement.DateFin:dd/MM/yyyy}. " +
                              $"Next one must start after this date."
                });
            }

            try
            {
                int capacity = await _reservationRepository.GetAvailableCapacityAsync(
                    abonnement.EspaceId, abonnement.DateDebut, abonnement.DateFin);

                abonnement.Statut = capacity > 0 ? Statut.Confirmé : Statut.Rejetée;
                if (capacity <= 0) abonnement.Notes = "Rejected: no available capacity.";
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            await _abonnementRepository.AddAsync(abonnement);
            await _abonnementRepository.SaveChangesAsync();

            return Ok(abonnement); // <-- success response returned
        }

        // PUT: api/Abonnement/{id} - Etudiant updates their own, Admin updates any
        [HttpPut("{id}")]
        [Authorize(Roles = "Etudiant,Admin")]
        public async Task<IActionResult> Update(int id, Abonnement abonnement)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var existing = await _abonnementRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Abonnement not found" });

            // If Etudiant: can only update their own abonnements
            if (userRole == "Etudiant" && existing.UtilisateurId != currentUserId)
            {
                return Forbid(); // 403 Forbidden
            }

            // If trying to update a confirmed abonnement with new dates, re-validate capacity
            if (existing.Statut == Statut.Confirmé &&
                (abonnement.DateDebut != existing.DateDebut || abonnement.DateFin != existing.DateFin))
            {
                try
                {
                    int availableCapacity = await _reservationRepository.GetAvailableCapacityAsync(
                        existing.EspaceId,
                        abonnement.DateDebut,
                        abonnement.DateFin
                    );

                    // Add back the current abonnement to the count since we're checking availability for the new dates
                    availableCapacity += 1;

                    if (availableCapacity <= 0)
                        return BadRequest(new { message = "No available capacity for the new time range." });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            // Update properties
            existing.Type = abonnement.Type;
            existing.Prix = abonnement.Prix;
            existing.DateDebut = abonnement.DateDebut;
            existing.DateFin = abonnement.DateFin;
            existing.Statut = abonnement.Statut;
            existing.Notes = abonnement.Notes;

            await _abonnementRepository.UpdateAsync(existing);
            await _abonnementRepository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Abonnement/{id} - Etudiant deletes their own, Admin deletes any
        [HttpDelete("{id}")]
        [Authorize(Roles = "Etudiant,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var abonnement = await _abonnementRepository.GetByIdAsync(id);
            if (abonnement == null)
                return NotFound(new { message = "Abonnement not found" });

            // If Etudiant: can only delete their own abonnements
            if (userRole == "Etudiant" && abonnement.UtilisateurId != currentUserId)
            {
                return Forbid(); // 403 Forbidden
            }

            await _abonnementRepository.DeleteAsync(abonnement);
            await _abonnementRepository.SaveChangesAsync();

            return NoContent();
        }
        // ================= GET abonnement types & prices =================
        [HttpGet("types")]
        [AllowAnonymous] // or keep restricted if needed
        public IActionResult GetAbonnementTypes()
        {
            return Ok(AbonnementTypeHelper.GetAllTypesWithPrices());
        }

    }
}