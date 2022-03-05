using Microsoft.EntityFrameworkCore;

namespace SignalRNet6.API.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
