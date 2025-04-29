using Microsoft.EntityFrameworkCore;
using StudentSync.Models;

namespace StudentSync.Data;

public class SQLiteDBContext : DbContext
{
    public SQLiteDBContext(DbContextOptions<SQLiteDBContext> options) :
        base(options)
    {
        
    }
    
    public DbSet<UserModel> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().ToTable("Users");
    }
    
}