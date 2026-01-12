using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CvProject.Models
{
    public class Experience
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Företagsnamn måste anges.")]
        [StringLength(100, ErrorMessage = "Företagsnamn får vara högst 100 tecken.")]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "Roll måste anges.")]
        [StringLength(100, ErrorMessage = "Roll får vara högst 100 tecken.")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Startdatum är obligatoriskt.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [StringLength(500, ErrorMessage = "Beskrivningen får vara högst 500 tecken.")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; } = string.Empty;
        public int CvId { get; set; }
        
        [ForeignKey(nameof(CvId))]
        public virtual Cv? Cv { get; set; }
    }
}
