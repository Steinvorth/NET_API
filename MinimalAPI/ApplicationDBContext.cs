using Microsoft.EntityFrameworkCore;
using MinimalAPI.Entities;

namespace MinimalAPI
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<Genre> Genres { get; set; }
    }    
}
