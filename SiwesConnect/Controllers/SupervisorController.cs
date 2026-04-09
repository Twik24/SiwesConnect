using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;

namespace SiwesConnect.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupervisorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewLogbookEntries()
        {
            var entries = _context.LogbookEntries.ToList();
            return View(entries);
        }

        public async Task<IActionResult> ApproveEntry(int id)
        {
            var entry = _context.LogbookEntries.FirstOrDefault(e => e.EntryId == id);
            if (entry != null)
            {
                entry.ApprovalStatus = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewLogbookEntries");
        }

        public async Task<IActionResult> RejectEntry(int id)
        {
            var entry = _context.LogbookEntries.FirstOrDefault(e => e.EntryId == id);
            if (entry != null)
            {
                entry.ApprovalStatus = "Rejected";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewLogbookEntries");
        }

        public IActionResult AddComment(int id)
        {
            var model = new ReviewViewModel { EntryId = id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(ReviewViewModel model)
        {
            var entry = _context.LogbookEntries.FirstOrDefault(e => e.EntryId == model.EntryId);
            if (entry != null)
            {
                entry.SupervisorComment = model.Comment;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewLogbookEntries");
        }
    }
}