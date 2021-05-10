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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Publications)
                .WithMany(p => p.Authors);

            modelBuilder.Entity<Publication>()
                .HasOne(p => p.Subject)
                .WithMany(s => s.Publications);
            modelBuilder.Entity<Publication>()
                .HasOne(p => p.Type)
                .WithMany(t => t.Publications);
            modelBuilder.Entity<Publication>()
                .HasMany(p => p.Authors)
                .WithMany(a => a.Publications);
        }
    }
}
