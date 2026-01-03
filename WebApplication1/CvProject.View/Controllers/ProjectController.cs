using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvProject.Models;
using CvProject.View.Models.Data;

namespace CvProject.View.Controllers
{
    public class ProjectController : Controller
    {
        private readonly MyAppContext _context;
        public ProjectController(MyAppContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return View(projects);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }
                project.CreatedDate = DateTime.Now;

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var project =  await _context.Projects.FindAsync(id);
            if (project == null)
            { 
                return NotFound(); 
            }
            return View(project);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(project);
            }
           var dbProject = await _context.Projects.FindAsync(id); 
            if (dbProject == null) 
            { 
                return NotFound(); 
            } 
            dbProject.Title = project.Title; 
            dbProject.Description = project.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
