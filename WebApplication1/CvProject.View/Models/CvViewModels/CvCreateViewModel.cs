using CvProject.Models;
using System.ComponentModel.DataAnnotations;

namespace CvProject.View.Models.CvViewModels
{
    public class CvCreateViewModel
    {
        public int Id { get; set; }
        public List<CvProject.Models.Education> Educations { get; set; } = new List<Education>();
        public List<CvProject.Models.Experience> Experiences { get; set; } = new List<Experience>();
        public List<CvProject.Models.Skill> Skills { get; set; } = new List<Skill>();
    }
}
