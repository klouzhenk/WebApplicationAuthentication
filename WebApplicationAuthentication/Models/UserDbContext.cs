using Microsoft.EntityFrameworkCore;
using WebApplicationAuthentication.Entities;

namespace WebApplicationAuthentication.Models
{

    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("app_users");
        }
    }
}
