using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CvProject.Models;

namespace CvProject.Models
{
    public class Cv
    {
        [Key]
        public int Id { get; set; }
        public virtual ICollection<Skill> Skills { get; set; }
        public virtual ICollection<Education> Educations { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; }
        public String UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
