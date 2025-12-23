using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CvProject.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Du måste ange ett namn.")]
        [StringLength(50)]
        [Display(Name = "Namn")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Privat profil")]
        public bool IsPrivate { get; set; }

        [Display(Name = "Profilbild")]
        public string? ProfilePictureUrl { get; set; }

        // Relation till kopplingstabellen
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}