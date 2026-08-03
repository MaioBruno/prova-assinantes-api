using Microsoft.EntityFrameworkCore;
using AssinantesApi.Data;
using AssinantesApi.Entities;
using AssinantesApi.Enums;

namespace AssinantesApi.Repositories
{
    public class AssinanteRepository : IAssinanteRepository
    {
        private readonly AppDbContext _context;

        public AssinanteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _context.Assinantes.AnyAsync(a => a.Email == email);
        }

        public async Task<bool> EmailExisteAsync(string email, Guid ignorarId)
        {
            return await _context.Assinantes.AnyAsync(a => a.Email == email && a.Id != ignorarId);
        }

        public async Task AdicionarAsync(Assinante assinante)
        {
            await _context.Assinantes.AddAsync(assinante);
        }

        public async Task<Assinante?> ObterPorIdAsync(Guid id)
        {
            return await _context.Assinantes.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Assinante?> ObterAtivoPorIdAsync(Guid id)
        {
            return await _context.Assinantes
                .FirstOrDefaultAsync(a => a.Id == id && a.Status == Status.Ativo);
        }

        public async Task<(int Total, IEnumerable<Assinante> Assinantes)> ListarAtivosAsync(int page, int size)
        {
            var query = _context.Assinantes
                .Where(a => a.Status == Status.Ativo);

            int total = await query.CountAsync();

            var assinantes = await query
                .OrderBy(a => a.NomeCompleto)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return (total, assinantes);
        }

        public Task RemoverAsync(Assinante assinante)
        {
            _context.Assinantes.Remove(assinante);
            return Task.CompletedTask;
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}