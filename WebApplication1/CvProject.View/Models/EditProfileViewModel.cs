using System.ComponentModel.DataAnnotations;

namespace CvProject.View.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Namn krävs")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-post krävs")]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Display(Name = "Adress")]
        public string? Address { get; set; }

        [Display(Name = "Privat profil")]
        public bool IsPrivate { get; set; }

        // Fält för att byta lösenord (valfritt)
        [DataType(DataType.Password)]
        [Display(Name = "Nuvarande lösenord (krävs endast vid byte av lösenord)")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta nytt lösenord")]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        public string? ConfirmNewPassword { get; set; }

        public List<ProjectSelectItem> AllProjects { get; set; } = new();
    }

    public class ProjectSelectItem
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

}

