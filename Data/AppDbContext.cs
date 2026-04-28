using AniTyan.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniTyan.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AnimeGirl> AnimeGirls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnimeGirl>()
                .ToTable("AnimeGirls")
                .HasKey(a => a.Id);
        }
    }
}
