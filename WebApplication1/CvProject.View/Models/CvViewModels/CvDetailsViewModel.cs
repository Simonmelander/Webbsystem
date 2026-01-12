using CvProject.Models;
using System.Collections.Generic;
using System;

namespace CvProject.View.Models.CvViewModels
{
    public class CvDetailsViewModel
    {
        private string? _profilePictureUrl;
        public string? ProfilePictureUrl
        {
            get => _profilePictureUrl;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    
                    _profilePictureUrl = "https://as1.ftcdn.net/jpg/00/57/04/58/1000_F_57045887_HHJml6DJVxNBMqMeDqVJ0ZQDnotp5rGD.jpg";
                }
                else
                {
                    _profilePictureUrl = value;
                }
            }
        }

        public int VisitCount { get; set; }
        public bool IsOwner { get; set; }
        public int Id { get; set; }
        public User User { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<EducationSummaryViewModel> Educations { get; set; } = new();
        public List<ExperienceSummaryViewModel> Experiences { get; set; } = new();
        public List<SkillSummaryViewModel> Skills { get; set; } = new();
        public List<ProjectSummaryViewModel> Projects { get; set; } = new();

        
        public List<SimilarPersonViewModel> SimilarCvProfiles { get; set; } = new();
    }

    public class SimilarPersonViewModel
    {
        public int CvId { get; set; }
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int MatchingSkillsCount { get; set; }
    }

    public class EducationSummaryViewModel
    {
        public int Id { get; set; }
        public string School { get; set; }
        public string FieldOfStudy { get; set; }
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

    public class ProjectSummaryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}