using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjProgFinalMVC2.Services;
using prjProgFinalMVC2.ViewModels;

namespace prjProgFinalMVC2.Controllers
{
    [Authorize(Roles = "HR,Coordinator")]
    public class LecturerController : Controller
    {
        private readonly ILecturerService _lecturerService;

        public LecturerController(ILecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        public async Task<IActionResult> Index()
        {
            var lecturers = await _lecturerService.GetLecturersAsync();
            return View(lecturers);
        }

        [Authorize(Roles = "HR")]
        public IActionResult Create()
        {
            return View(new CreateLecturerViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Create(CreateLecturerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _lecturerService.CreateLecturerAsync(model);
                if (result)
                {
                    TempData["Success"] = "Lecturer registered successfully";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to register lecturer");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _lecturerService.DeleteLecturerAsync(id);
            if (result)
            {
                TempData["Success"] = "Lecturer deleted successfully";
            }
            else
            {
                TempData["Error"] = "Failed to delete lecturer";
            }
            return RedirectToAction(nameof(Index));
        }
    }



}
