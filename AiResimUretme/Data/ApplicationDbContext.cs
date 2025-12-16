using Microsoft.EntityFrameworkCore;
using AiResimUretme.Models;

namespace AiResimUretme.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AiImage> AiImages { get; set; }
    }
}