namespace CvProject.Models
{
    // Data Transfer Objects (DTOs) för att exportera XML
    public class CvExportDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<EducationExportDto> Educations { get; set; } = new();
        public List<ExperienceExportDto> Experiences { get; set; } = new();
        public List<SkillExportDto> Skills { get; set; } = new();
        public List<ProjectExportDto> Projects { get; set; } = new();
    }

    public class EducationExportDto
    {
        public string School { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class ExperienceExportDto
    {
        public string Company { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class SkillExportDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class ProjectExportDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
    }
