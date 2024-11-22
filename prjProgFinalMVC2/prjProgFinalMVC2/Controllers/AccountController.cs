using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using prjProgFinalMVC2.Services;
using prjProgFinalMVC2.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace prjProgFinalMVC2.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await _authService.LoginAsync(model);
                if (!string.IsNullOrEmpty(token))
                {
                    HttpContext.Session.SetString("JWTToken", token);
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value),
                    new Claim(ClaimTypes.Role, jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value),
                    new Claim("UserId", jwtToken.Claims.First(c => c.Type == "UserId").Value)
                };
                    var identity = new ClaimsIdentity(claims, "Cookies");
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync("Cookies", principal);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }


}
