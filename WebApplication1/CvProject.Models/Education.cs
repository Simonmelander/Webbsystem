using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CvProject.Models
{
    public class Education
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Skolans namn måste anges")]
        [StringLength(100, ErrorMessage = "Skolans namn får vara högst 100 tecken")]
        [Display(Name = "Skola/Universitet")]
        public string School { get; set; } = string.Empty;

        [Required(ErrorMessage = "Examen/Titel måste anges.")]
        [StringLength(100, ErrorMessage = "Examen får vara högst 100 tecken.")]
        [Display(Name = "Examen")]
        public string Degree { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ämne/Inriktning måste anges.")]
        [StringLength(100)]
        [Display(Name = "Studieområde")]
        public string FieldOfStudy { get; set; } = string.Empty;

        [Required(ErrorMessage = "Startdatum är obligatoriskt.")]
        [DataType(DataType.Date)]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Slutdatum")]
        public DateTime? EndDate { get; set; }
        

        [StringLength(10, ErrorMessage = "Betyget får högst vara 10 tecken")]
        [Display(Name = "Betyg")]
        public string? Grade { get; set; }

        [StringLength(500, ErrorMessage = "Beskrivningen får vara högst 500 tecken.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Beskrivning")]
        public string? Description { get; set; } = string.Empty;

        public int CvId { get; set; }

        [ForeignKey(nameof(CvId))]
        public virtual Cv? Cv { get; set; }

    }
}
