using Microsoft.EntityFrameworkCore;
using AssinantesApi.Data;
using AssinantesApi.Entities;
using AssinantesApi.DTOs;
using AssinantesApi.Enums;

namespace AssinantesApi.Services
{
    public class AssinanteService : IAssinanteService
    {
        private readonly AppDbContext _context;

        public AssinanteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AssinanteResponseDTO> CriarAsync(AssinanteCreateDTO dto)
        {
            bool emailEmUso = await _context.Assinantes
                .AnyAsync(a => a.Email == dto.Email);

            if (emailEmUso)
                throw new ArgumentException("Este e-mail já está cadastrado no sistema.");

            var assinante = new Assinante(
                dto.NomeCompleto,
                dto.Email,
                dto.DataInicioAssinatura,
                dto.Plano,
                dto.ValorMensal
            );

            _context.Assinantes.Add(assinante);
            await _context.SaveChangesAsync();

            return MapToResponse(assinante);
        }

        public async Task<(int Total, IEnumerable<AssinanteResponseDTO> Assinantes)> ListarTodosAsync(int page, int size)
        {
            var query = _context.Assinantes
                .Where(a => a.Status == Status.Ativo);

            int totalRegistros = await query.CountAsync();

            var assinantes = await query
                .OrderBy(a => a.NomeCompleto)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(a => MapToResponse(a))
                .ToListAsync();

            return (totalRegistros, assinantes);
        }

        public async Task<AssinanteResponseDTO?> ObterPorIdAsync(Guid id)
        {
            var a = await _context.Assinantes
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Ativo);

            return a == null ? null : MapToResponse(a);
        }

        public async Task AtualizarParcialAsync(Guid id, AssinantePatchDTO dto)
        {
            var a = await _context.Assinantes.FindAsync(id);

            if (a == null || a.Status != Status.Ativo)
                throw new KeyNotFoundException("Assinante ativo não encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.NomeCompleto))
                a.SetNome(dto.NomeCompleto);

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                bool emailEmUso = await _context.Assinantes
                    .AnyAsync(x => x.Email == dto.Email && x.Id != id);

                if (emailEmUso)
                    throw new ArgumentException("E-mail já está em uso.");

                a.SetEmail(dto.Email);
            }

            if (dto.Plano.HasValue)
                a.SetPlano(dto.Plano.Value);

            if (dto.ValorMensal.HasValue)
                a.SetValorMensal(dto.ValorMensal.Value);

            if (dto.Status.HasValue)
            {
                if (dto.Status == Status.Ativo)
                    a.Ativar();
                else
                    a.Desativar();
            }

            if (dto.DataInicioAssinatura.HasValue)
                a.SetDataInicio(dto.DataInicioAssinatura.Value);

            await _context.SaveChangesAsync();
        }

        public async Task DesativarAsync(Guid id)
        {
            var assinante = await _context.Assinantes.FindAsync(id);

            if (assinante == null)
                throw new KeyNotFoundException("Assinante não encontrado.");

            assinante.Desativar();

            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(Guid id)
        {
            var assinante = await _context.Assinantes.FindAsync(id);

            if (assinante == null)
                throw new KeyNotFoundException("Assinante não encontrado.");

            _context.Assinantes.Remove(assinante);
            await _context.SaveChangesAsync();
        }

        private static AssinanteResponseDTO MapToResponse(Assinante a)
        {
            return new AssinanteResponseDTO
            {
                Id = a.Id,
                NomeCompleto = a.NomeCompleto,
                Email = a.Email,
                DataInicioAssinatura = a.DataInicioAssinatura,
                Plano = a.Plano,
                ValorMensal = a.ValorMensal,
                Status = a.Status,
                TempoDeAssinaturaEmMeses = a.TempoDeAssinaturaEmMeses
            };
        }
    }
}