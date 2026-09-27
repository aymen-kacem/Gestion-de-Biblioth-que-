using CoworkingGes.DTO;
using CoworkingGes.Models;
using CoworkingGes.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoworkingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceRepository _maintenanceRepository;

        public MaintenanceController(IMaintenanceRepository maintenanceRepository)
        {
            _maintenanceRepository = maintenanceRepository;
        }

        // =================== ADMIN ONLY ===================

        // GET: api/Maintenance - Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var maintenances = await _maintenanceRepository.GetAllAsync();
            return Ok(maintenances);
        }

        // GET: api/Maintenance/{id} - Admin only
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var maintenance = await _maintenanceRepository.GetByIdAsync(id);
            if (maintenance == null)
                return NotFound(new { message = "Maintenance not found" });

            return Ok(maintenance);
        }

        // GET: api/Maintenance/espace/{espaceId} - Admin only
        [HttpGet("espace/{espaceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByEspace(int espaceId)
        {
            var maintenances = await _maintenanceRepository.GetByEspaceIdAsync(espaceId);
            return Ok(maintenances);
        }
        // GET Technicien
        [HttpGet("technicien/my-maintenances")]
        [Authorize(Roles = "Technicien")]
        public async Task<IActionResult> GetMyMaintenances()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var maintenances = await _maintenanceRepository.GetByTechnicienIdAsync(currentUserId);
            return Ok(maintenances);
        }

        // =================== ADMIN + TECHNICIEN ===================

        // POST: api/Maintenance - Technicien creates their own, Admin can create for anyone
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MaintenanceDTO maintenance)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            // If Admin: can create for anyone (default to admin if not set)
             if (userRole == "Admin" && maintenance.TechnicienId == 0)
            {
                maintenance.TechnicienId = currentUserId;
            }

            await _maintenanceRepository.AddAsync(maintenance);
            await _maintenanceRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = maintenance.TechnicienId }, maintenance);
        }

        // PUT: api/Maintenance/{id} - Technicien updates their own, Admin updates any
        [HttpPut("{id}")]
        [Authorize(Roles = "Technicien,Admin")]
        public async Task<IActionResult> Update(int id, Maintenance updated)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var existing = await _maintenanceRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Maintenance not found" });

            // If Technicien: can only update their own maintenance
            if (userRole == "Technicien" && existing.TechnicienId != currentUserId)
                return Forbid();

            existing.Statut = updated.Statut;
            existing.Description = updated.Description;
            existing.Date = updated.Date;

            await _maintenanceRepository.UpdateAsync(existing);
            await _maintenanceRepository.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Maintenance/{id} - Technicien deletes their own, Admin deletes any
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var maintenance = await _maintenanceRepository.GetByIdAsync(id);
            if (maintenance == null)
                return NotFound(new { message = "Maintenance not found" });
            // ✅ Actually delete it!
            await _maintenanceRepository.DeleteAsync(maintenance);
            await _maintenanceRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
