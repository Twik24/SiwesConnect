using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;

namespace SiwesConnect.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Register()
        {
            ViewBag.Companies = _context.Companies
                .Where(c => c.Status == "Active")
                .ToList();
            return View();  
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    UserName = model.Email,
                    UserType = model.UserType,
                    CompanyID = model.CompanyID
                };

                var result = await _userManager.CreateAsync(user, model.Password !);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, user.UserType!);
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.Companies = _context.Companies
             .Where(c => c.Status == "Active")
            .ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email!,
                    model.Password!,
                    model.RememberMe,
                    false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email!);
                    var roles = await _userManager.GetRolesAsync(user!);

                    if (roles.Contains("Admin"))
                        return RedirectToAction("Index", "Admin");
                    else if (roles.Contains("Supervisor"))
                        return RedirectToAction("Index", "Supervisor");
                    else
                        return RedirectToAction("Index", "Student");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }


        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}