using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext>options) : base(options)
        { 
        }
    }
}
