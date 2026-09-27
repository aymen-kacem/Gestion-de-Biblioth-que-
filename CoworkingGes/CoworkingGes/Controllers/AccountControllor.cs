using CoworkingGes.DTO;
using CoworkingGes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoworkingGes.Enum;

namespace CoworkingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Utlisateur> userManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<Utlisateur> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            if (await userManager.FindByEmailAsync(register.Username) != null)
                return BadRequest("User already exists");

            var user = new Utlisateur
            {
                UserName = register.Username,
                Email = register.EmailAddress,
                Role = UserRole.Etudiant // Always Etudiant
            };

            var result = await userManager.CreateAsync(user, register.Password);
            if (result.Succeeded) return Created("", null);

            return BadRequest("Problem creating user");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var user = await userManager.FindByNameAsync(login.Username);
            if (user == null || !await userManager.CheckPasswordAsync(user, login.Password))
                return Unauthorized("Invalid username or password");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:issuer"],
                audience: configuration["JWT:audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                username = login.Username,
                role = user.Role.ToString()
            });
        }

        [HttpGet("techniciens")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTechniciens()
        {
            var techniciens = userManager.Users
                .Where(u => u.Role == CoworkingGes.Enum.UserRole.Technicien)
                .ToList();

            return Ok(techniciens);
        }
        // Add this method after GetAllTechniciens in your AccountController

        [HttpGet("all-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = userManager.Users
                    .Select(u => new
                    {
                        id = u.Id,
                        userName = u.UserName,
                        email = u.Email,
                        role = u.Role.ToString()
                    })
                    .ToList();

                if (users == null || users.Count == 0)
                    return Ok(new List<object>());

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error fetching users: " + ex.Message });
            }
        }
        // PUT: api/Account/change-role/{userId}
        // Only Admin can change roles
        [HttpPut("change-role/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserRole(string userId, [FromBody] ChangeUserRoleDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required." });

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            try
            {
                user.Role = dto.NewRole;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return BadRequest(new { message = $"Failed to update role: {errors}" });
                }

                return Ok(new { message = $"User role updated to {dto.NewRole}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating role: {ex.Message}" });
            }
        }


    }
}
