using CvProject.Models;
using CvProject.View.Models.Data;
using Microsoft.EntityFrameworkCore;
using CvProject.View.Models.ViewModels;


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
            try
            {
                return await _db.Projects
                    .Include(p => p.Creator)
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving projects from the database.", ex);
            }
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            try
            {
                return await _db.Projects.FindAsync(id);
            }
            catch
            {
                throw new Exception("Error retrieving project by ID from the database.");
            }
        }

        public async Task CreateAsync(Project project, string creatorId)
        {

            project.CreatedDate = DateTime.Now;
            project.CreatorId = creatorId;

            if (creatorId == null)
            {
                throw new Exception("CreatorId cannot be null when creating a project.");
            }
            try
            {
                _db.Projects.Add(project);
                await _db.SaveChangesAsync();
            }
            catch
            (Exception ex)
            {
                throw new Exception("Error creating new project in the database.", ex);
            }
        }

        public async Task<bool> UpdateAsync(int id, Project edited, string currentUserId)
        {

            var dbProject = await GetByIdAsync(id);

            if (dbProject == null) return false;

            if (dbProject.CreatorId != currentUserId) return false;

            dbProject.Title = edited.Title;
            dbProject.Description = edited.Description;

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating project in the database.", ex);
            }
        }
        public async Task<bool> DeleteAsync(int projectId, string currentUserId)
        {
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return false;

            if (project.CreatorId != currentUserId) return false;

            var links = await _db.ProjectUsers
                .Where(pu => pu.ProjectId == projectId)
                .ToListAsync();

            if (links.Count > 0)
                _db.ProjectUsers.RemoveRange(links);

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> JoinProjectAsync(int projectId, string userId)
        {
            try
            {
                var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
                if (!projectExists) return false;

                var alreadyJoined = await _db.ProjectUsers
                    .AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);

                if (alreadyJoined) return true;

                var link = new ProjectUser
                {
                    ProjectId = projectId,
                    UserId = userId
                };

                _db.ProjectUsers.Add(link);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error joining project in the database.", ex);
            }
        }

        public async Task<bool> AddMemberAsLeaderAsync(int projectId, string memberUserId, string leaderUserId)
        {
            try
            {
                var project = await GetByIdAsync(projectId);
                if (project == null) return false;

                if (project.CreatorId != leaderUserId) return false;

                var exists = await _db.ProjectUsers
                    .AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == memberUserId);

                if (exists) return true;

                _db.ProjectUsers.Add(new ProjectUser
                {
                    ProjectId = projectId,
                    UserId = memberUserId
                });

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding member to project in the database.", ex);
            }
        }

        public async Task<bool> RemoveMemberAsLeaderAsync(int projectId, string memberUserId, string leaderUserId)
        {
           try
            {
                var project = await _db.Projects.FindAsync(projectId);
                if (project == null) return false;

                if (project.CreatorId != leaderUserId) return false;

                var link = await _db.ProjectUsers
                    .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == memberUserId);

                if (link == null) return true;

                _db.ProjectUsers.Remove(link);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing member from project in the database.", ex);
            }
        }

        public async Task<ProjectDetailsViewModel?> GetProjectDetailsAsync(int projectId, string? currentUserId)
        {
            try
            {
                var project = await _db.Projects
                .Include(p => p.Creator)
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)
                .FirstOrDefaultAsync(p => p.Id == projectId);

                if (project == null) return null;

                bool isSignedIn = !string.IsNullOrWhiteSpace(currentUserId);
                bool isLeader = isSignedIn && project.CreatorId == currentUserId;

                var members = project.ProjectUsers
                    .Where(pu => pu.User != null)
                    .Select(pu => new ProjectMemberItem
                    {
                        UserId = pu.UserId,
                        Name = pu.User!.Name,
                        IsPrivate = pu.User!.IsPrivate
                    })
                    .ToList();

                var model = new ProjectDetailsViewModel
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    CreatedDate = project.CreatedDate,
                    CreatorId = project.CreatorId,
                    CreatorName = project.Creator?.Name ?? "Okänd",
                    IsSignedIn = isSignedIn,
                    IsLeader = isLeader,
                    Members = members
                };

                if (isLeader)
                {
                    var memberIds = members.Select(m => m.UserId).ToList();

                    model.AddableUsers = await _db.Users
                        .Where(u => !memberIds.Contains(u.Id))
                        .Select(u => new UserOptionItem
                        {
                            UserId = u.Id,
                            Name = u.Name
                        })
                        .ToListAsync();
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving project details from the database.", ex);
            }
        }
    }
}
