using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentSync.Models;

namespace StudentSync.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add your custom tables here
        public DbSet<Student> Students { get; set; }
        public DbSet<Consumer> Consumers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important: keep this to configure Identity

            modelBuilder.Entity<Student>()
                .ToTable("Student")
                .HasKey(s => s.StudentNumber);

            modelBuilder.Entity<Consumer>()
                .Property(c => c.ConsumerId)
                .IsRequired()
                .HasMaxLength(10);

            // Add other custom configurations as needed
        }
    }
}