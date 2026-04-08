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
                var internship = new Internship
                {
                    Title = model.Title,
                    Description = model.Description,
                    Location = model.Location,
                    Duration = model.Duration,
                    ApplicationDeadline = model.ApplicationDeadline,
                    DatePosted = DateTime.Now
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
        public async Task<IActionResult> Apply(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var existingApplication = _context.Applications
                .FirstOrDefault(a => a.InternshipID == id && a.StudentID == user!.Id);

            if (existingApplication != null)
            {
                return Content("You have already applied for this internship.");
            }

            var application = new Application
            {
                InternshipID = id,
                StudentID = user!.Id,
                ApplicationDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Content("Application submitted successfully!");
        }
    }
}