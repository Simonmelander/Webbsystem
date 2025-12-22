using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Models.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext>options) : base(options)
        { 
        }
        public DbSet<WebApplication1.Models.User> User { get; set; } = default!;
    }
}
