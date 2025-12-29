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
        public string Company { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public int CvId { get; set; }
        
        [ForeignKey(nameof(CvId))]
        public virtual Cv? Cv { get; set; }
    }
}
