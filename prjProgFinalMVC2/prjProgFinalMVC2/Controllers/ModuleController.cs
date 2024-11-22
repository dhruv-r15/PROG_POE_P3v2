using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjProgFinalMVC2.Services;
using prjProgFinalMVC2.ViewModels;

namespace prjProgFinalMVC2.Controllers
{
    [Authorize(Roles = "HR")]
    public class ModuleController : Controller
    {
        private readonly IModuleService _moduleService;

        public ModuleController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        public async Task<IActionResult> Index()
        {
            var modules = await _moduleService.GetModulesAsync();
            return View(modules);
        }

        public IActionResult Create() => View(new CreateModuleViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(CreateModuleViewModel model)
        {
            if (ModelState.IsValid && await _moduleService.CreateModuleAsync(model))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _moduleService.DeleteModuleAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }







}
