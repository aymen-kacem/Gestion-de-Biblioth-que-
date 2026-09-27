using CoworkingGes.DTO;
using CoworkingGes.Models;
using CoworkingGes.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoworkingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class RessourceController : ControllerBase
    {
        private readonly IRessourceRepository _ressourceRepository;

        public RessourceController(IRessourceRepository ressourceRepository)
        {
            _ressourceRepository = ressourceRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAll()
        {
            var ressources = await _ressourceRepository.GetAllAsync();
            return Ok(ressources);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetById(int id)
        {
            var ressource = await _ressourceRepository.GetByIdAsync(id);
            if (ressource == null)
                return NotFound();
            return Ok(ressource);
        }

        [HttpGet("espace/{espaceId}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetByEspace(int espaceId)
        {
            var ressources = await _ressourceRepository.GetByEspaceIdAsync(espaceId);
            return Ok(ressources);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RessourceDTO ressource)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User not authenticated" });

            if (!int.TryParse(userIdClaim, out int currentUserId))
                return BadRequest(new { message = "Invalid user ID" });

            // Automatically assign the current user
            ressource.UtilisateurId = currentUserId;

            await _ressourceRepository.AddAsync(ressource);
            await _ressourceRepository.SaveChangesAsync();
            return Created();
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, Ressource updated)
        {
            var existing = await _ressourceRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            // Update all properties
            existing.Nom = updated.Nom;
            existing.Type = updated.Type;
            existing.QuantiteDisponible = updated.QuantiteDisponible;
            existing.EspaceId = updated.EspaceId;  // ✅ Add this line

            await _ressourceRepository.UpdateAsync(existing);
            await _ressourceRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]


        public async Task<IActionResult> Delete(int id)
        {
            var ressource = await _ressourceRepository.GetByIdAsync(id);
            if (ressource == null)
                return NotFound();

            await _ressourceRepository.DeleteAsync(ressource);
            await _ressourceRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
