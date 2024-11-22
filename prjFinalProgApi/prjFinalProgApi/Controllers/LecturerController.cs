using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjFinalProgApi.Data;
using prjFinalProgApi.DTOs;
using prjFinalProgApi.Models;

namespace prjFinalProgApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LecturerController : ControllerBase
    {
        private readonly ClaimsDbContext _context;

        public LecturerController(ClaimsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "HR,Coordinator")]
        public async Task<ActionResult<IEnumerable<Lecturer>>> GetLecturers()
        {
            return await _context.Lecturers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Lecturer>> GetLecturer(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.Claims)
                .FirstOrDefaultAsync(l => l.LecturerId == id);

            if (lecturer == null)
                return NotFound();

            return lecturer;
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult<Lecturer>> CreateLecturer(LecturerCreateDTO dto)
        {
            var lecturer = new Lecturer
            {
                Username = dto.Username,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLecturer), new { id = lecturer.LecturerId }, lecturer);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer == null) return NotFound();

            _context.Lecturers.Remove(lecturer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }


}
