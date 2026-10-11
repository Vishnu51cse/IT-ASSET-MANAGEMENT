using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MVCFrontend.Models;

namespace MVCFrontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new MvcLoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(MvcLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.LoginAsync(new LoginDto { Email = model.Email, Password = model.Password });
            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email address or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Email, result.Email),
                new Claim(ClaimTypes.Role, result.Role),
                new Claim(ClaimTypes.Name, result.Name)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
