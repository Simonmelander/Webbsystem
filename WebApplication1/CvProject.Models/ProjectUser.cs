using System;
using System.Collections.Generic;
using System.Text;

namespace CvProject.Models
{
    public class ProjectUser
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
