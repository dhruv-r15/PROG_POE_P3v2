using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjFinalProgApi.Data;
using prjFinalProgApi.Models;

namespace prjFinalProgApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClaimsController : ControllerBase
    {
        private readonly ClaimsDbContext _context;

        public ClaimsController(ClaimsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetClaims()
        {
            return await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Module)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Claim>> SubmitClaim(Claim claim)
        {
            claim.Status = "Pending";
            claim.SubmissionDate = DateTime.UtcNow;

            var module = await _context.Modules.FindAsync(claim.ModuleId);
            claim.TotalAmount = claim.HoursWorked * module.HourlyRate;

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClaims), new { id = claim.ClaimId }, claim);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Claim>> GetClaim(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Module)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            return claim == null ? NotFound() : claim;
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Coordinator,HR")]
        public async Task<IActionResult> UpdateClaimStatus(int id, string status)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = status;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }



}
