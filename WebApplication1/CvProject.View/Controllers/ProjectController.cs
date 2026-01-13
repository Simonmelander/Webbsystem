using CvProject.Models;
using CvProject.View.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvProject.View.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectService _projectService;
        private readonly UserManager<User> _userManager;

        public ProjectController(ProjectService projectService, UserManager<User> userManager)
        {
            _projectService = projectService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return View(projects);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (!ModelState.IsValid) return View(project);

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            await _projectService.CreateAsync(project, userId);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(currentUserId)) return Unauthorized();

            bool ok = await _projectService.DeleteAsync(id, currentUserId);

            if (!ok) return Forbid();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            if (project.CreatorId != userId) return Forbid();

            return View(project);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id) return BadRequest();
            if (!ModelState.IsValid) return View(project);

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            bool ok = await _projectService.UpdateAsync(id, project, userId);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            bool ok = await _projectService.JoinProjectAsync(id, userId);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(int id, string userId)
        {
            var leaderId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(leaderId)) return Unauthorized();

            bool ok = await _projectService.AddMemberAsLeaderAsync(id, userId, leaderId);
            if (!ok) return Forbid();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(int id, string userId)
        {
            var leaderId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(leaderId)) return Unauthorized();

            bool ok = await _projectService.RemoveMemberAsLeaderAsync(id, userId, leaderId);
            if (!ok) return Forbid();

            return RedirectToAction(nameof(Details), new { id });
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = _userManager.GetUserId(User);

            var model = await _projectService.GetProjectDetailsAsync(id, currentUserId);
            if (model == null) return NotFound();

            return View(model);
        }



    }
}
