using System.ComponentModel.DataAnnotations;

namespace CvProject.View.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Användarnamn krävs")]
        [Display(Name = "Användarnamn")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lösenord krävs")]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }
    }
}