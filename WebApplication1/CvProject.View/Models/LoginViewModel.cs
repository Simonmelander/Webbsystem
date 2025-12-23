using System.ComponentModel.DataAnnotations;

namespace CvProject.View.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Användarnamn krävs")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lösenord krävs")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }
    }
}