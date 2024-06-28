using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication16.Models;

namespace WebApplication16.DataBase
{
    public class ClassContext:IdentityDbContext
    {
        public ClassContext(DbContextOptions<ClassContext> options):base(options)
        {
            
        } 
        public DbSet<AppUser> appUsers { get; set; }
    }
}
