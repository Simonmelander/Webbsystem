using System.ComponentModel.DataAnnotations;

namespace CvProject.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Du måste ange ett namn.")]
        [StringLength(50, ErrorMessage = "Namnet får inte vara längre än 50 tecken.")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Adressen får inte vara längre än 100 tecken.")]
        [Display(Name = "Adress")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Du måste ange en e-postadress.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Ogiltig e-postadress.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Du måste ange ett lösenord.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Lösenordet måste vara minst 8 tecken långt och innehålla minst en stor bokstav, en liten bokstav, en siffra och ett specialtecken.")]
        public string Password { get; set; }

        [Display(Name = "Telefonnummer")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Ogiltigt telefonnummer.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Privat profil")]
        public bool IsPrivate { get; set; }
        
        [Display(Name = "Profilbild")]
        public string ProfilePictureUrl { get; set; }
        public virtual ICollection<ProjectUser> ProjectUser { get; set; } = new List<ProjectUser>();
    }
}
