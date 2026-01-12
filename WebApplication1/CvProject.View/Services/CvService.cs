using CvProject.Models;
using CvProject.View.Models.CvViewModels;
using CvProject.View.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace CvProject.View.Services
{
    public class CvService
    {
        private readonly MyAppContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CvService(MyAppContext db, IWebHostEnvironment hostingEnvironment)
        {
            _context = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task CreateCvAsync(CvCreateViewModel viewModel, string userId, IFormFile? profileImage)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new Exception("Användaren hittades inte.");
                }

                if (profileImage != null && profileImage.Length > 0)
                {
                    try
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
                    catch (Exception ex)
                    {
                        throw new Exception("Ett fel uppstod vid uppladdning av profilbild: " + ex.Message);
                    }
                }

                var newCv = new Cv
                {
                    UserId = userId,
                };

                AssignValidEntriesToCv(viewModel, newCv);

                _context.Cvs.Add(newCv);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ett fel uppstod vid skapandet av CV: " + ex.Message);
            }
        }

        public async Task<CvDetailsViewModel?> GetCvDetailsAsync(int cvId, string? currentUserId)
        {
            Cv? cv = await GetCvAsync(cvId);

            if (cv == null) return null;

            if (!cv.User.IsActive) return null;


            if (cv.User.IsPrivate && cv.UserId != currentUserId)
            {
                return null;
            }

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
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new Exception("Användaren hittades inte.");
                }

                Cv? cv = await GetCvAsync(cvId);
                if (cv == null || cv.UserId != userId)
                {
                    return false;
                }

                if (profileImage != null && profileImage.Length > 0)
                {
                    try
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
                    catch (Exception ex)
                    {
                        throw new Exception("Ett fel uppstod vid uppladdning av profilbild: " + ex.Message);
                    }

                }

                AssignValidEntriesToCv(viewModel, cv);
                _context.Update(cv);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ett fel uppstod vid uppdatering av CV: " + ex.Message);
            }
        }

        public async Task<bool> DeleteCvAsync(int cvId, string userId)
        {
            Cv? cv = await GetCvAsync(cvId);
            if (cv == null || cv.UserId != userId) return false;
            _context.Cvs.Remove(cv);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task IncrementCvVisitsAsync(int cvId)
        {
            try
            {
                Cv? cv = await GetCvAsync(cvId);
                if (cv == null) throw new Exception("CV not found.");

                cv.Visits++;
                _context.Update(cv);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while incrementing CV visits.");
            }
        }

        public async Task<List<SimilarPersonViewModel>> GetSimilarCvsAsync(int currentCvId, bool isAuthenticated)
        {
            var currentCv = await _context.Cvs
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == currentCvId);

            if (currentCv == null || !currentCv.Skills.Any())
                return new List<SimilarPersonViewModel>();

            var currentSkillNames = currentCv.Skills
                .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .Select(s => s.Name.ToLower().Trim())
                .ToList();


            var otherCvs = await _context.Cvs
                .Include(c => c.User)
                .Include(c => c.Skills)
                .Where(c => c.Id != currentCvId && !c.User.IsPrivate)
                .ToListAsync();

            var similarProfiles = new List<SimilarPersonViewModel>();

            foreach (var otherCv in otherCvs)
            {
                int matchCount = otherCv.Skills
                    .Count(s => !string.IsNullOrWhiteSpace(s.Name) &&
                                currentSkillNames.Contains(s.Name.ToLower().Trim()));

                if (matchCount > 0)
                {
                    similarProfiles.Add(new SimilarPersonViewModel
                    {
                        CvId = otherCv.Id,
                        FullName = otherCv.User.Name,
                        ProfilePictureUrl = otherCv.User.ProfilePictureUrl,
                        MatchingSkillsCount = matchCount
                    });
                }
            }

            return similarProfiles
                .OrderByDescending(x => x.MatchingSkillsCount)
                .Take(3)
                .ToList();
        }

        /// <summary>
        /// Assigns only valid entries from the ViewModel to the Cv entity.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="cv"></param>
        private static void AssignValidEntriesToCv(CvCreateViewModel viewModel, Cv cv)
        {
            if (cv.Educations != null) cv.Educations.Clear();
            if (cv.Experiences != null) cv.Experiences.Clear();
            if (cv.Skills != null) cv.Skills.Clear();

            cv.Educations = viewModel.Educations.Where(e => !string.IsNullOrWhiteSpace(e.School)).ToList();
            cv.Experiences = viewModel.Experiences.Where(e => !string.IsNullOrWhiteSpace(e.Company)).ToList();
            cv.Skills = viewModel.Skills.Where(s => !string.IsNullOrWhiteSpace(s.Name)).ToList();
        }

        private async Task<Cv?> GetCvAsync(int cvId)
        {
            try
            {
                return await _context.Cvs
    .Include(c => c.User)
    .ThenInclude(u => u.ProjectUsers)
    .ThenInclude(pu => pu.Project)
    .Include(c => c.Educations)
    .Include(c => c.Experiences)
    .Include(c => c.Skills)
    .FirstOrDefaultAsync(c => c.Id == cvId);
            }
            catch (Exception)
            {
                throw new Exception("Ett fel uppstod vid hämtning av CV.");
            }
        }

        public async Task<string> GetCvXmlAsync(int cvId, string? currentUserId)
        {
            var cv = await GetCvDetailsAsync(cvId, currentUserId);

            if (cv == null) return string.Empty;

            var exportData = new CvExportDto
            {
                FullName = cv.User.Name,
                Email = cv.User.Email ?? string.Empty,

                Experiences = cv.Experiences.Select(e => new ExperienceExportDto
                {
                    Company = e.Company,
                    Position = e.Position,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description
                }).ToList(),

                Educations = cv.Educations.Select(e => new EducationExportDto
                {
                    School = e.School,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Description = e.Description
                }).ToList(),

                Skills = cv.Skills.Select(s => new SkillExportDto
                {
                    Name = s.Name
                }).ToList(),

                Projects = cv.User.ProjectUsers.Select(p => new ProjectExportDto
                {
                    Title = p.Project.Title,
                    Description = p.Project.Description
                }).ToList()
            };

            return SerializationService.SerializeToXML(exportData);
        }

    }
}