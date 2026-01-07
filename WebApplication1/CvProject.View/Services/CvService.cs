using CvProject.Models;
using CvProject.View.Models.CvViewModels;
using CvProject.View.Models.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CvProject.View.Services
{
    public class CvService
    {
        private readonly MyAppContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CvService(MyAppContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task CreateCvAsync(CvCreateViewModel viewModel, string userId, IFormFile? profileImage)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (profileImage != null && profileImage.Length > 0 && user != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + profileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }
                user.ProfilePictureUrl = "/images/" + uniqueFileName;
            }

            var newCv = new Cv
            {
                UserId = userId,
            };

            AssignValidEntriesToCv(viewModel, newCv);

            _db.Cvs.Add(newCv);
            await _db.SaveChangesAsync();
        }

        public async Task<CvDetailsViewModel?> GetCvDetailsAsync(int cvId, string? currentUserId)
        {
            Cv? cv = await GetCvAsync(cvId);

            if (cv == null) return null;

            return new CvDetailsViewModel
            {
                Id = cv.Id,
                FullName = cv.User.Name,
                User = cv.User,
                Email = cv.User.Email ?? string.Empty,
                IsOwner = cv.UserId == currentUserId,
                ProfilePictureUrl = cv.User.ProfilePictureUrl,
                VisitCount = cv.Visits,

                Educations = cv.Educations.Select(e => new EducationSummaryViewModel
                {
                    School = e.School,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description
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
                }).ToList(),

                Projects = cv.User.ProjectUsers.Select(p => new ProjectSummaryViewModel
                {
                    Title = p.Project.Title,
                    Description = p.Project.Description,
                    CreatedDate = p.Project.CreatedDate,
                }).ToList()
            };
        }


        public async Task<CvCreateViewModel?> GetCvForEditAsync(int cvId, string userId)
        {
            Cv? cv = await GetCvAsync(cvId);

            if (cv == null || cv.UserId != userId) return null;

            return new CvCreateViewModel
            {
                Id = cv.Id,
                Educations = cv.Educations.ToList(),
                Experiences = cv.Experiences.ToList(),
                Skills = cv.Skills.ToList(),
                User = cv.User,
                ProfilePictureUrl = cv.User.ProfilePictureUrl
            };
        }

        public async Task<bool> UpdateCvAsync(int cvId, CvCreateViewModel viewModel, string userId, IFormFile? profileImage)
        {
            Cv? cv = await GetCvAsync(cvId);
            if (cv == null || cv.UserId != userId) return false;

            if (profileImage != null && profileImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + profileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }

                cv.User.ProfilePictureUrl = "/images/" + uniqueFileName;
            }

            AssignValidEntriesToCv(viewModel, cv);

            _db.Update(cv);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCvAsync(int cvId, string userId)
        {
            Cv? cv = await GetCvAsync(cvId);
            if (cv == null || cv.UserId != userId) return false;
            _db.Cvs.Remove(cv);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task IncrementCvVisitsAsync(int cvId)
        {
            Cv? cv = await GetCvAsync(cvId);
            if (cv != null)
            {
                cv.Visits++;
                _db.Update(cv);
                await _db.SaveChangesAsync();
            }
        }

        // likande profiler
        public async Task<List<SimilarPersonViewModel>> GetSimilarCvsAsync(int currentCvId)
        {
            // 1. Hämta nuvarande CV och skills
            var currentCv = await _db.Cvs
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == currentCvId);

            if (currentCv == null || !currentCv.Skills.Any())
                return new List<SimilarPersonViewModel>();

            // Gör om skills till små bokstäver för enkel jämförelse
            var currentSkillNames = currentCv.Skills
                .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .Select(s => s.Name.ToLower().Trim())
                .ToList();

            // 2. Hämta alla andra CV:n som INTE är privata
            var otherCvs = await _db.Cvs
                .Include(c => c.User)
                .Include(c => c.Skills)
                .Where(c => c.Id != currentCvId && c.User.IsPrivate == false)
                .ToListAsync();

            var matches = new List<SimilarPersonViewModel>();

            foreach (var otherCv in otherCvs)
            {
                // Räkna hur många skills som matchar
                int count = otherCv.Skills
                    .Count(s => !string.IsNullOrWhiteSpace(s.Name) &&
                                currentSkillNames.Contains(s.Name.ToLower().Trim()));

                if (count > 0)
                {
                    matches.Add(new SimilarPersonViewModel
                    {
                        CvId = otherCv.Id,
                        FullName = otherCv.User.Name,
                        ProfilePictureUrl = otherCv.User.ProfilePictureUrl, // Kan vara null, hanteras i Vyn
                        MatchingSkillsCount = count
                    });
                }
            }

            // Sortera: Flest matchningar först, ta max 3
            return matches.OrderByDescending(x => x.MatchingSkillsCount).Take(3).ToList();
        }


        private static void AssignValidEntriesToCv(CvCreateViewModel viewModel, Cv cv)
        {
            cv.Educations = viewModel.Educations.Where(e => !string.IsNullOrWhiteSpace(e.School)).ToList();
            cv.Experiences = viewModel.Experiences.Where(e => !string.IsNullOrWhiteSpace(e.Company)).ToList();
            cv.Skills = viewModel.Skills.Where(s => !string.IsNullOrWhiteSpace(s.Name)).ToList();
        }

        private async Task<Cv?> GetCvAsync(int cvId)
        {
            return await _db.Cvs
                .Include(c => c.User)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == cvId);
        }
    }
}