using Microsoft.EntityFrameworkCore;
using StudentSync.Models;

namespace StudentSync.Data
{
    public class SQLiteDBContext : DbContext
    {
        // Constructor with strongly-typed DbContextOptions
        public SQLiteDBContext(DbContextOptions<SQLiteDBContext> options) : base(options)
        {
        }

        // DbSet property for Students table
        public DbSet<Student> Students { get; set; }
        public DbSet<Consumer> Consumers { get; set; }

        // Configure the model (optional step for further customization)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .ToTable("Student") // Explicitly set the table name
                .HasKey(s => s.StudentNumber); // Optional, to ensure 'StudentNumber' is the PK

            // Additional configuration if needed
            modelBuilder.Entity<Consumer>()
                .Property(s => s.ConsumerId)
                .IsRequired() // Make sure the StudentNumber is required
                .HasMaxLength(10); // Optionally, set length restrictions
            // Add more model configurations as needed
        }
    }
}
