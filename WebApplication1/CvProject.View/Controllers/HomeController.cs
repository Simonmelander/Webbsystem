using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CvProject.Models;
using CvProject.View.Models.Data;
using Microsoft.EntityFrameworkCore;
using CvProject.View.Models;
using System.Linq;
using System.Threading.Tasks;

namespace CvProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyAppContext _context;

        public HomeController(MyAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featured = await _context.Cvs
                .Include(c => c.User)
                .Where(c => c.User != null && !c.User.IsPrivate && c.User.IsActive)
                .OrderByDescending(c => c.Id)
                .Take(5)
                .ToListAsync();

            var latestProject = await _context.Projects
                .OrderByDescending(p => p.CreatedDate)
                .FirstOrDefaultAsync();

            var vm = new HomeViewModel
            {
                FeaturedCvs = featured,
                LatestProject = latestProject
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
