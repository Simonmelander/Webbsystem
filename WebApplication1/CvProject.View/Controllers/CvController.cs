using CvProject.Models;
using CvProject.View.Models;
using CvProject.View.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CvProject.View.Controllers
{
    public class CvController : Controller
    {

        private readonly MyAppContext _db;

        public CvController(MyAppContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CvCreateViewModel();

            // VIKTIGT: Vi lägger till EN tom rad av varje typ.
            // Då kommer formuläret visa fält för en utbildning, ett jobb, etc.
            model.Educations.Add(new Education());
            model.Experiences.Add(new Experience());
            model.Skills.Add(new Skill());

            // Vill du ha plats för 3 skills direkt? Lägg till fler:
            model.Skills.Add(new Skill());
            model.Skills.Add(new Skill());

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CvCreateViewModel model)
        {
            // Eftersom du inte har Titel eller UserId i formuläret än:
            // Hämta UserId från inloggningen (hårdkodat till "1" tills vidare)
            string currentUserId = "1";

            // Vi måste städa bort tomma rader som användaren inte fyllde i
            // T.ex. om man lämnade "Skola" tomt, ska det inte sparas.
            var validEducations = model.Educations.Where(e => !string.IsNullOrEmpty(e.School)).ToList();
            var validExperiences = model.Experiences.Where(e => !string.IsNullOrEmpty(e.Company)).ToList();
            var validSkills = model.Skills.Where(s => !string.IsNullOrEmpty(s.Name)).ToList();

            var newCv = new Cv
            {
                UserId = currentUserId,
                Educations = validEducations,
                Experiences = validExperiences,
                Skills = validSkills
            };

            _db.Cvs.Add(newCv);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index)); // Eller Details
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
            return View(cv);
        }
    }
}
