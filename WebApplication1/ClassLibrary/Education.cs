using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClassLibrary
{
    public class Education
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string School { get; set; }
        [Required]
        public string Degree { get; set; }
        public string FieldOfStudy { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [StringLength(20)]
        public string Grade { get; set; }
        public string Description { get; set; }

        public int CvId { get; set; }

        [ForeignKey(nameof(CvId))]
        public virtual Cv Cv { get; set; }

    }
}
