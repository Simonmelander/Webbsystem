using CvProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CvProject.View.Models.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext>options) : base(options)
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurera Sender-relationen
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany() // Eller .WithMany(u => u.SentMessages) om du har en lista i User
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Detta stoppar "Multiple Cascade Paths"

            // Konfigurera Receiver-relationen
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany() // Eller .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
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
