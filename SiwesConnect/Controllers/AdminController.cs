
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;

namespace SiwesConnect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        

        

        
        }
    }
