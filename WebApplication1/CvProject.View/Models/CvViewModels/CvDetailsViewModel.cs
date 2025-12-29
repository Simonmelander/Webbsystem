using CvProject.Models;

namespace CvProject.View.Models.CvViewModels
{
    public class CvDetailsViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<EducationSummaryViewModel> Educations { get; set; } = new();
        public List<ExperienceSummaryViewModel> Experiences { get; set; } = new();
        public List<SkillSummaryViewModel> Skills { get; set; } = new();
    }

    public class EducationSummaryViewModel
    {
        public int Id { get; set; }
        public string School { get; set; }
        public string Degree { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }

    public class ExperienceSummaryViewModel
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class SkillSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
