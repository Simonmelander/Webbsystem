using CvProject.Models;
using CvProject.View.Models.CvViewModels;
using CvProject.View.Models.Data;
using CvProject.View.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CvProject.View.Controllers
{
    public class CvController : Controller
    {
        private readonly CvService _cvService;
        private readonly UserManager<User> _userManager;
        private readonly MyAppContext _context;

        public CvController(CvService cvService, UserManager<User> userManager, MyAppContext context)
        {
            _cvService = cvService;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            var model = new CvCreateViewModel();
            model.Educations.Add(new Education());
            model.Experiences.Add(new Experience());
            model.Skills.Add(new Skill());
            model.Skills.Add(new Skill());
            model.Skills.Add(new Skill());
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CvCreateViewModel viewModel, IFormFile? profileImage)
        {
            string? currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(currentUserId)) return Unauthorized();

            if (!ModelState.IsValid) return View(viewModel);

            await _cvService.CreateCvAsync(viewModel, currentUserId, profileImage);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            var model = await _cvService.GetCvDetailsAsync(id, userId);

            if (model == null) return NotFound();

            if (!model.IsOwner)
            {
                await _cvService.IncrementCvVisitsAsync(id);
                model.VisitCount += 1;
            }

            model.SimilarCvProfiles = await _cvService.GetSimilarCvsAsync(id);

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var model = await _cvService.GetCvForEditAsync(id, userId);

            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CvCreateViewModel model, IFormFile? profileImage)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            bool success = await _cvService.UpdateCvAsync(id, model, userId, profileImage);

            if (!success) return NotFound();

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            bool success = await _cvService.DeleteCvAsync(id, userId);

            if (!success) return NotFound();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Index()
        {
            
            var allCvs = await _context.Cvs
                .Include(c => c.User)
                .Where(c => !c.User.IsPrivate && c.User.IsActive)
                .ToListAsync();

            return View(allCvs);
        }

        public async Task<IActionResult> Search(string searchString)
        {
            
            var cvQuery = _context.Cvs
                .Include(c => c.User)
                .Include(c => c.Skills)
                .Where(c => !c.User.IsPrivate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                
                var searchTerms = searchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                
                foreach (var term in searchTerms)
                {
                    /
                    cvQuery = cvQuery.Where(c =>
                        c.User.Name.Contains(term) ||
                        c.User.UserName.Contains(term) ||
                        c.Skills.Any(s => s.Name.Contains(term))
                    );
                }
            }

            return View("Index", await cvQuery.ToListAsync());
        }

        public async Task<IActionResult> DownloadXml(int id)
        {
            var userId = _userManager.GetUserId(User);

            string? xmlData = await _cvService.GetCvXmlAsync(id, userId);

            if (xmlData == null) return NotFound();

            return File(System.Text.Encoding.UTF8.GetBytes(xmlData), "application/xml", "cv_export.xml");
        }
    }
}