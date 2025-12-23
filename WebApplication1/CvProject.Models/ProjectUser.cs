namespace CvProject.Models
{
    public class ProjectUser
    {
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}