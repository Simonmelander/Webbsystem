namespace CvProject.View.Models.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public string CreatorName { get; set; } = string.Empty;
        public string? CreatorId { get; set; }

        public bool IsSignedIn { get; set; }
        public bool IsLeader { get; set; }

        public List<ProjectMemberItem> Members { get; set; } = new();
        public List<UserOptionItem> AddableUsers { get; set; } = new();
    }

    public class ProjectMemberItem
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
    }

    public class UserOptionItem
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
