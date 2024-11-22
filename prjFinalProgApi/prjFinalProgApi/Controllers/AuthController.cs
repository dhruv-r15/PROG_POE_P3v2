using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prjFinalProgApi.Data;
using prjFinalProgApi.DTOs;
using prjFinalProgApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Claim = prjFinalProgApi.Models.Claim;

namespace prjFinalProgApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ClaimsDbContext _context;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, ClaimsDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                switch (model.Role)
                {
                    case "Lecturer":
                        var lecturer = new Lecturer
                        {
                            Username = model.Username,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
                        };
                        _context.Lecturers.Add(lecturer);
                        break;

                    case "Coordinator":
                        var coordinator = new Coordinator
                        {
                            Username = model.Username,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
                        };
                        _context.Coordinators.Add(coordinator);
                        break;
                }

                await _context.SaveChangesAsync();
                return Ok(new { Message = "User registered successfully" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginDto model)
        {
            // HR Manager login
            if (model.Username == _configuration["AdminCredentials:Username"] &&
                model.Password == _configuration["AdminCredentials:Password"])
            {
                return new AuthResponse
                {
                    Token = GenerateJwtToken("HR", "0", model.Username),
                    Role = "HR",
                    Username = model.Username
                };
            }

            // Lecturer login
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Username == model.Username);
            if (lecturer != null && BCrypt.Net.BCrypt.Verify(model.Password, lecturer.PasswordHash))
            {
                return new AuthResponse
                {
                    Token = GenerateJwtToken("Lecturer", lecturer.LecturerId.ToString(), lecturer.Username),
                    Role = "Lecturer",
                    Username = lecturer.Username,
                    UserId = lecturer.LecturerId.ToString()
                };
            }

            // Coordinator login
            var coordinator = await _context.Coordinators
                .FirstOrDefaultAsync(c => c.Username == model.Username);
            if (coordinator != null && BCrypt.Net.BCrypt.Verify(model.Password, coordinator.PasswordHash))
            {
                return new AuthResponse
                {
                    Token = GenerateJwtToken("Coordinator", coordinator.CoordinatorId.ToString(), coordinator.Username),
                    Role = "Coordinator",
                    Username = coordinator.Username,
                    UserId = coordinator.CoordinatorId.ToString()
                };
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string role, string userId, string username)
        {
            var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(ClaimTypes.Name, username),
            new System.Security.Claims.Claim(ClaimTypes.Role, role),
            new System.Security.Claims.Claim("UserId", userId)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
