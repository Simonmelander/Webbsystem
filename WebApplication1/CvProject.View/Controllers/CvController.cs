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

        public CvController(CvService cvService, UserManager<User> userManager)
        {
            _cvService = cvService;
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
        [Authorize]
        public async Task<IActionResult> Create(CvCreateViewModel viewModel)
        {
            string? currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(currentUserId)) return Unauthorized();

            if(!ModelState.IsValid) return View(viewModel);

            await _cvService.CreateCvAsync(viewModel, currentUserId);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _cvService.GetCvDetailsAsync(id, userId);

            if (model == null) return NotFound();

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
        public async Task<IActionResult> Edit(int id, CvCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            bool success = await _cvService.UpdateCvAsync(id, model, userId);

            if (!success) return NotFound();

            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            bool success = await _cvService.DeleteCvAsync(id, userId);
            
            if (!success) return NotFound();
            return RedirectToAction("Index", "Home");
        }

    }
}
