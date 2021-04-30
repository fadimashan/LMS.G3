using Microsoft.EntityFrameworkCore;
using LMS.API.Models.Entities;

namespace LMS.API.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Author> Authors  { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<PublicationType> PublicationTypes { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
    }
}
