using Microsoft.EntityFrameworkCore;
using AssinantesApi.Entities;

namespace AssinantesApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Assinante> Assinantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Assinante>(entity =>
            {
                entity.Property(a => a.ValorMensal)
                      .HasPrecision(18, 2);

                entity.HasIndex(a => a.Email)
                      .IsUnique();

                entity.Property(a => a.NomeCompleto)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(a => a.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(a => a.Status)
                      .HasConversion<int>();

                entity.Property(a => a.Plano)
                      .HasConversion<int>();
            });
        }
    }
}