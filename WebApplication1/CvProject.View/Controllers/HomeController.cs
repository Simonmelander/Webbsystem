using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CvProject.Models;
using CvProject.View.Models.Data;
using Microsoft.EntityFrameworkCore;
using CvProject.View.Models;

namespace CvProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyAppContext _context;

        public HomeController(MyAppContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var vm = new HomeViewModel
            {
                FeaturedCvs = _context.Cvs
                    .Include(c => c.User)
                    .Where(c => c.User != null && !c.User.IsPrivate) 
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .ToList(),

                LatestProject = _context.Projects
                    .OrderByDescending(p => p.CreatedDate)
                    .FirstOrDefault()
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
