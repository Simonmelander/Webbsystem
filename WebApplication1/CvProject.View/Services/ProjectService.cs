using CvProject.Models;
using CvProject.View.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace CvProject.View.Services
{
    public class ProjectService
    {
        private readonly MyAppContext _db;

        public ProjectService(MyAppContext db)
        {
            _db = db;
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await _db.Projects
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _db.Projects.FindAsync(id);
        }

        public async Task CreateAsync(Project project, string creatorId)
        {
            project.CreatedDate = DateTime.Now;
            project.CreatorId = creatorId;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, Project edited, string currentUserId)
        {
            var dbProject = await _db.Projects.FindAsync(id);
            if (dbProject == null) return false;

            if (dbProject.CreatorId != currentUserId) return false;

            dbProject.Title = edited.Title;
            dbProject.Description = edited.Description;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
