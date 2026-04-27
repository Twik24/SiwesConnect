using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;

namespace SiwesConnect.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Show form to post internship
        [Authorize(Roles = "Supervisor")]
        public IActionResult PostInternship()
        {
            return View();
        }

        // Handle internship form submission
        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> PostInternship(InternshipViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var internship = new Internship
                {
                    Title = model.Title,
                    Description = model.Description,
                    Location = model.Location,
                    Duration = model.Duration,
                    ApplicationDeadline = model.ApplicationDeadline,
                    DatePosted = DateTime.Now,
                    SupervisorID = user!.Id
                };

                _context.Internships.Add(internship);
                await _context.SaveChangesAsync();

                return RedirectToAction("BrowseInternships");
            }

            return View(model);
        }

        // Students browse internships
        [Authorize]
        public async Task<IActionResult> BrowseInternships()
        {
            var internships = _context.Internships.ToList();
            return View(internships);
        }

        //Students apply for the internship
        [Authorize(Roles = "Student")]
        public IActionResult Apply(int id)
        {
            var model = new ApplyViewModel { InternshipID = id };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Apply(ApplyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var existingApplication = _context.Applications
                    .FirstOrDefault(a => a.InternshipID == model.InternshipID && a.StudentID == user!.Id);

                if (existingApplication != null)
                {
                    return RedirectToAction("AlreadyApplied");
                }

                var application = new Application
                {
                    InternshipID = model.InternshipID,
                    StudentID = user!.Id,
                    ApplicationDate = DateTime.Now,
                    Status = "Pending",
                    CoverLetter = model.CoverLetter
                };

                _context.Applications.Add(application);
                await _context.SaveChangesAsync();

                return RedirectToAction("ApplicationSuccess");
            }

            return View(model);
        }


        [Authorize(Roles = "Student")]
        public IActionResult ApplicationSuccess()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult AlreadyApplied()
        {
            return View();
        }
        // Show company registration form (public - no login needed)
        [AllowAnonymous]
        public IActionResult RegisterCompany()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCompany(CompanyRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var company = new Company
                {
                    CompanyName = model.CompanyName,
                    Industry = model.Industry,
                    Location = model.Location,
                    ContactEmail = model.ContactEmail,
                    PhoneNumber = model.PhoneNumber,
                    Status = "Pending"
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                return RedirectToAction("CompanyRegistrationSuccess");
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult CompanyRegistrationSuccess()
        {
            return View();
        }
    }
  }
