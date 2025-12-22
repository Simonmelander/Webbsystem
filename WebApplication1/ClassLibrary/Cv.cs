using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

namespace ClassLibrary
{
    public class Cv
    {
        [Key]
        public int Id { get; set; }
        public virtual ICollection<Skill> Skills { get; set; }
        public virtual ICollection<Education> Educations { get; set; }
        public virtual ICollection<string> Experiences { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
