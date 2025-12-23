using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CvProject.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Du måste ange ett namn.")]
        [StringLength(50)]
        [Display(Name = "Namn")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Adress")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Privat profil")]
        public bool IsPrivate { get; set; } = false;

        [Display(Name = "Profilbild")]
        public string? ProfilePictureUrl { get; set; }

        // Relationer
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}