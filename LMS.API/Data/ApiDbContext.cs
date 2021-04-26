using LMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Author> Authors  { get; set; }
        public DbSet<Literature> Literatures { get; set; }
        
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
    }
}