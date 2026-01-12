using CvProject.Models;
using CvProject.View.Models;
using CvProject.View.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CvProject.View.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace CvProject.View.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly MyAppContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, MyAppContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Name = model.Name,
                        Address = model.Address ?? string.Empty,
                        IsPrivate = false
                    };


                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }


                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }


                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ett oväntat fel inträffade. Vänligen försök igen senare.");
                return View(model);
            }
        }

        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    if (user != null && !user.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "Detta konto har avslutats.");
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(
                        model.UserName,
                        model.Password,
                        model.RememberMe,
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Felaktigt användarnamn eller lösenord.");
                }
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ett oväntat fel inträffade. Vänligen försök igen senare.");
                return View(model);
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new EditProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                IsPrivate = user.IsPrivate
            };

            var userId = user.Id;

            var allProjects = await _context.Projects
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            var userProjectIds = await _context.ProjectUsers
                .Where(pu => pu.UserId == userId)
                .Select(pu => pu.ProjectId)
                .ToListAsync();

            model.AllProjects = allProjects.Select(p => new ProjectSelectItem
            {
                ProjectId = p.Id,
                Title = p.Title,
                IsSelected = userProjectIds.Contains(p.Id)
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userId = user.Id;

            if (!ModelState.IsValid)
            {
                await LoadProjectsAsync(model, userId);
                return View(model);
            }

            user.Name = model.Name;
            user.Email = model.Email;
            user.Address = model.Address ?? string.Empty;
            user.IsPrivate = model.IsPrivate;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await LoadProjectsAsync(model, userId);
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.CurrentPassword))
            {
                var passwordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    await LoadProjectsAsync(model, userId);
                    return View(model);
                }
            }

            var selectedProjectIds = model.AllProjects
                .Where(p => p.IsSelected)
                .Select(p => p.ProjectId)
                .ToList();

            var existingLinks = await _context.ProjectUsers
                .Where(pu => pu.UserId == userId)
                .ToListAsync();

            _context.ProjectUsers.RemoveRange(existingLinks);

            foreach (var projectId in selectedProjectIds)
            {
                _context.ProjectUsers.Add(new ProjectUser
                {
                    UserId = userId,
                    ProjectId = projectId
                });
            }

            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);

            TempData["Message"] = "Din profil har uppdaterats!";
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateAccount()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }


                user.IsActive = false;


                user.IsPrivate = true;

                await _userManager.UpdateAsync(user);


                await _signInManager.SignOutAsync();

                TempData["Message"] = "Ditt konto har avslutats.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData["Error"] = "Ett oväntat fel inträffade. Vänligen försök igen senare.";
                return RedirectToAction("EditProfile");
            }
        }
        private async Task LoadProjectsAsync(EditProfileViewModel model, string userId)
        {
            try
            {
                var allProjects = await _context.Projects
                               .OrderByDescending(p => p.CreatedDate)
                               .ToListAsync();

                var userProjectIds = await _context.ProjectUsers
                    .Where(pu => pu.UserId == userId)
                    .Select(pu => pu.ProjectId)
                    .ToListAsync();

                model.AllProjects = allProjects.Select(p => new ProjectSelectItem
                {
                    ProjectId = p.Id,
                    Title = p.Title,
                    IsSelected = userProjectIds.Contains(p.Id)
                }).ToList();
            }
            catch (Exception)
            {
                model.AllProjects = new List<ProjectSelectItem>();
            }
        }
    }
}