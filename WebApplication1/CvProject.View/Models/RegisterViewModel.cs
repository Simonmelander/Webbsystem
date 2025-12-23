using System.ComponentModel.DataAnnotations;

namespace CvProject.View.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Användarnamn krävs")]
        [Display(Name = "Användarnamn")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lösenord krävs")]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Namn krävs")] // För din User-modell
        public string Name { get; set; }

        [Display(Name = "Epost")]   
        public string Email { get; set; }

        [Display(Name = "Adress")] 
        public string Address { get; set; }
    }
}