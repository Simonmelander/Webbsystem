using CvProject.Models;
using CvProject.View.Models.CvViewModels;
using CvProject.View.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace CvProject.View.Services
{
    public class CvService
    {
        private readonly MyAppContext _db;

        public CvService(MyAppContext db)
        {
            _db = db;
        }

        public async Task CreateCvAsync(CvCreateViewModel viewModel, string userId)
        {
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
                Email = cv.User.Email ?? string.Empty,
                IsOwner = cv.UserId == currentUserId,
                ProfilePictureUrl = cv.User.ProfilePictureUrl,

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
                Skills = cv.Skills.ToList()
            };
        }

        public async Task<bool> UpdateCvAsync(int cvId, CvCreateViewModel viewModel, string userId)
        {
            Cv? cv = await GetCvAsync(cvId);
            if (cv == null || cv.UserId != userId) return false;

            

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
