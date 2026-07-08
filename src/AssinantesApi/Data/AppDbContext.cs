using Microsoft.EntityFrameworkCore;
using AssinantesApi.Entities;

namespace AssinantesApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Isso aqui cria a tabela no banco de dados baseada na sua classe Assinante
        public DbSet<Assinante> Assinantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        // Aqui resolvemos o aviso do 'ValorMensal'
        modelBuilder.Entity<Assinante>()
            .Property(a => a.ValorMensal)
            .HasPrecision(18, 2); // Define: 18 dígitos totais, sendo 2 após a vírgula
        }
    }
}