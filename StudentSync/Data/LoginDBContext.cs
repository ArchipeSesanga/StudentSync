using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudentSync.Data;

public class LoginDBContext : IdentityDbContext
{
    public LoginDBContext(DbContextOptions<LoginDBContext> options) 
        : base(options)
    {
        
    }
}