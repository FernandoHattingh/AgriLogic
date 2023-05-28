using AgriLogic.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriLogic.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Farmer> Farmers { get; set; } // DbSet for the Farmer entity

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Farmer>().ToTable("Farmer");

        }
    }
}
