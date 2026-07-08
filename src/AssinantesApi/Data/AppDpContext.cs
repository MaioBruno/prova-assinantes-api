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
    }
}