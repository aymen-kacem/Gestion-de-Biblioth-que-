using CoworkingGes.DTO;
using CoworkingGes.Models;
using CoworkingGes.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoworkingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class EspaceController : ControllerBase
    {
        private readonly IEspaceRepository _espaceRepository;
        private readonly Context _context;

        public EspaceController(IEspaceRepository espaceRepository, Context context)
        {
            _espaceRepository = espaceRepository;
            _context = context;  // ✅ Add this
        }




        [HttpGet]
        [Authorize(Roles = "Admin, Etudiant")]

        public async Task<IActionResult> GetAll()
        {
            var espaces = await _espaceRepository.GetAllAsync();
            return Ok(espaces);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetById(int id)
        {
            var espace = await _espaceRepository.GetByIdAsync(id);
            if (espace == null)
                return NotFound();
            return Ok(espace);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create(EspaceDTO espace)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();
            int currentUserId = int.Parse(userIdClaim);
            // If Etudiant: can only create for themselves
            if (userRole == "Admin")
            {
                if (espace.UtilisateurId != 0 && espace.UtilisateurId != currentUserId)
                {
                    return Forbid(); // 403 Forbidden
                }
                espace.UtilisateurId = currentUserId; // Force to their own ID
            }
            await _espaceRepository.AddAsync(espace);
            await _espaceRepository.SaveChangesAsync();
            return Created();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Update(int id, Espace espace)
        {
            var existing = await _espaceRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.Nom = espace.Nom;
            existing.Type = espace.Type;
            existing.Capacite = espace.Capacite;
            existing.Localisation = espace.Localisation;

            await _espaceRepository.UpdateAsync(existing);
            await _espaceRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var espace = await _espaceRepository.GetByIdAsync(id);
            if (espace == null)
                return NotFound();

            // ✅ Check if collections exist AND have items
            if (espace.Ressources?.Any() == true)
                return BadRequest(new { message = "Cannot delete Espace with associated Ressources." });

            if (espace.Reservations?.Any() == true)
                return BadRequest(new { message = "Cannot delete Espace with associated Reservations." });

            if (espace.Maintenances?.Any() == true)
                return BadRequest(new { message = "Cannot delete Espace with associated Maintenances." });

            if (espace.Abonnements?.Any() == true)
                return BadRequest(new { message = "Cannot delete Espace with associated Abonnements." });

            await _espaceRepository.DeleteAsync(espace);
            await _espaceRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
