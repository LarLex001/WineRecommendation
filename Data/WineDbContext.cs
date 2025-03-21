using Microsoft.EntityFrameworkCore;
using WineRecommendation.Models;

namespace WineRecommendation.Data
{
    public class WineDbContext : DbContext
    {
        public WineDbContext(DbContextOptions<WineDbContext> options)
            : base(options)
        {
        }

        public DbSet<WineData> Wines { get; set; }
        public DbSet<WinePredictionResult> PredictionResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WineData>()
                .HasKey(w => w.Id);

            modelBuilder.Entity<WineData>()
                .Property(w => w.Type)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<WinePredictionResult>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<WinePredictionResult>()
                .Property(p => p.PredictedType)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}