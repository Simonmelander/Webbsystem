using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CvProject.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Färdighetens namn måste anges.")]
        [StringLength(50, ErrorMessage = "Färdighetens namn får vara högst 50 tecken.")]
        public string Name { get; set; }
        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public virtual Cv? Cv { get; set; }
    }
}
