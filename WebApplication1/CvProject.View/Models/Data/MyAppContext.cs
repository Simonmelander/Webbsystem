using CvProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CvProject.View.Models.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext>options) : base(options)
        { 
        }
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Cv> Cvs { get; set; } = default!;
        public DbSet<Project> Projects { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;

        public DbSet<Skill> Skills { get; set; } = default!;
        public DbSet<Experience> Experiences { get; set; } = default!;
        public DbSet<Education> Educations { get; set; } = default!;

    }
}
