using CvProject.Models;
using System.ComponentModel.DataAnnotations;

namespace CvProject.View.Models
{
    public class CvCreateViewModel
    {
        public List<Education> Educations { get; set; } = new List<Education>();
        public List<Experience> Experiences { get; set; } = new List<Experience>();
        public List<Skill> Skills { get; set; } = new List<Skill>();
    }
}
