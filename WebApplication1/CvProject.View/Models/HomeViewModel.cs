using System.Collections.Generic;
using CvProject.Models;

namespace CvProject.View.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Cv> FeaturedCvs { get; set; } = new List<Cv>();
        public Project? LatestProject { get; set; }
    }
}