using CvProject.Models;
using CvProject.View.Models.CvViewModels;
using CvProject.View.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvProject.View.Controllers
{
    public class CvController : Controller
    {

        private readonly MyAppContext _db;
        private readonly UserManager<User> _userManager;

        public CvController(MyAppContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
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
        public IActionResult Create(CvCreateViewModel model)
        {
            string? currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(currentUserId)) return Unauthorized();

            // Vi måste städa bort tomma rader som användaren inte fyllde i
            // T.ex. om man lämnade "Skola" tomt, ska det inte sparas.
            var validEducations = model.Educations.Where(e => !string.IsNullOrEmpty(e.School)).ToList();
            var validExperiences = model.Experiences.Where(e => !string.IsNullOrEmpty(e.Company)).ToList();
            var validSkills = model.Skills.Where(s => !string.IsNullOrEmpty(s.Name)).ToList();

            var newCv = new CvProject.Models.Cv
            {
                UserId = currentUserId,
                Educations = validEducations,
                Experiences = validExperiences,
                Skills = validSkills
            };

            _db.Cvs.Add(newCv);
            _db.SaveChanges();

            return RedirectToAction("Index", "Home"); // Eller Details
        }

        public IActionResult Details(int id)
        {
            var cv = _db.Cvs
                .Include(c => c.Skills)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == id);

            if (cv == null)
            {
                return NotFound();
            }

            var model = new CvDetailsViewModel
            {
                Id = cv.Id,
                FullName = cv.User.Name,
                Email = cv.User.Email,

                Educations = cv.Educations.Select(e => new EducationSummaryViewModel
                {
                    School = e.School,
                    Degree = e.Degree,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description,
                }).ToList(),

                Experiences = cv.Experiences.Select(e => new ExperienceSummaryViewModel
                {
                    Company = e.Company,
                    Position = e.Position,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description
                }).ToList(),

                Skills = cv.Skills.Select(s => new SkillSummaryViewModel
                {
                    Name = s.Name,
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var cv = _db.Cvs
                .Include(c => c.Skills)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .FirstOrDefault(c => c.Id == id);

            if (cv == null) return NotFound();

            var model = new CvCreateViewModel
            {
                Id = cv.Id,
                Educations = cv.Educations.ToList(),
                Experiences = cv.Experiences.ToList(),
                Skills = cv.Skills.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, CvCreateViewModel model)
        {
            var cv = _db.Cvs
                .Include(c => c.Skills)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .FirstOrDefault(c => c.Id == id);

            if (cv == null) return NotFound();

            if (!ModelState.IsValid)
            {
                Console.Write("ModelState is invalid:");
                return View(model);
            }

            // Rensa bort gamla poster
            _db.Educations.RemoveRange(cv.Educations);
            _db.Experiences.RemoveRange(cv.Experiences);
            _db.Skills.RemoveRange(cv.Skills);

            cv.Educations = model.Educations.Where(e => !string.IsNullOrEmpty(e.School)).ToList();
            cv.Experiences = model.Experiences.Where(e => !string.IsNullOrEmpty(e.Company)).ToList();
            cv.Skills = model.Skills.Where(s => !string.IsNullOrEmpty(s.Name)).ToList();

            try
            {
                _db.Update(cv);
                _db.SaveChanges();
                return RedirectToAction("Details", new { id = cv.Id });
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Ett fel uppstod vid uppdatering av CV:t. Försök igen.");
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var cv = await _db.Cvs.FindAsync(id);
            if (cv == null) return NotFound();
            
            var currentUserId = _userManager.GetUserId(User);
            if (cv.UserId != currentUserId) return Forbid();

            _db.Cvs.Remove(cv);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

    }
}
