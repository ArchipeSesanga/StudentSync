using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudentSync.Data;

public class LoginDBContext : IdentityDbContext<IdentityUser>
{
    public LoginDBContext(DbContextOptions<LoginDBContext> options) 
        : base(options)
    {
        
    }
}