using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using prjProgFinalMVC2.Services;
using prjProgFinalMVC2.ViewModels;
using prjProgFinalMVC2.Models;
using System.Net.Http;

namespace prjProgFinalMVC2.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IModuleService _moduleService;
        private readonly IDocumentService _documentService;

        public ClaimsController(IClaimService claimService, IModuleService moduleService, IDocumentService documentService)
        {
            _claimService = claimService;
            _moduleService = moduleService;
            _documentService = documentService;
        }

        public async Task<IActionResult> Index()
        {
            var claims = await _claimService.GetClaimsAsync();
            return View(claims);
        }

        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Create()
        {
            await SetupModulesViewBag();
            return View(new CreateClaimViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Create(CreateClaimViewModel model)
        {
            var module = await _moduleService.GetModuleAsync(model.ModuleId);
            var userId = User.FindFirst("UserId")?.Value;

            model.LecturerId = int.Parse(userId);
            model.TotalAmount = module.HourlyRate * model.HoursWorked;

            var result = await _claimService.SubmitClaimAsync(model);
            if (result.Success && result.ClaimId.HasValue)
            {
                if (model.Document != null)
                {
                    await _documentService.UploadDocumentAsync(result.ClaimId.Value, model.Document);
                }
                return RedirectToAction(nameof(Index));
            }

            await SetupModulesViewBag();
            return View(model);
        }

        private async Task SetupModulesViewBag()
        {
            var modules = await _moduleService.GetModulesAsync();
            ViewBag.Modules = new SelectList(modules, "ModuleId", "ModuleName");
        }

        public async Task<IActionResult> Details(int id)
        {
            var claim = await _claimService.GetClaimAsync(id);
            return claim == null ? NotFound() : View(claim);
        }

        [HttpPost]
        [Authorize(Roles = "HR,Coordinator")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var result = await _claimService.UpdateClaimStatusAsync(id, status);
            return Json(new { success = result });
        }

        public async Task<IActionResult> DownloadDocument(int id)
        {
            var document = await _documentService.GetDocumentAsync(id);
            if (document == null) return NotFound();

            return File(document.FileContent, "application/pdf", document.FileName);
        }

    }





}
