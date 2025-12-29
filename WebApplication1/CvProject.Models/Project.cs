using System.ComponentModel.DataAnnotations;

namespace CvProject.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Du måste ange titel på projektet.")]
        [MaxLength(50, ErrorMessage = "Titeln får inte vara längre än 50 tecken.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Du måste ange en beskrivning av projektet.")]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        
        public string? CreatorId { get; set; }
        public virtual User? Creator { get; set; }

        
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}