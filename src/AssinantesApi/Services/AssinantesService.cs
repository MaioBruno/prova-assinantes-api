using AssinantesApi.DTOs;
using AssinantesApi.Entities;
using AssinantesApi.Enums;
using AssinantesApi.Repositories;
using AssinantesApi.Factories;

namespace AssinantesApi.Services
{
    public class AssinanteService : IAssinanteService
    {
        private readonly IAssinanteRepository _repository;
        private readonly IAssinanteFactory _factory;

        public AssinanteService(IAssinanteRepository repository, IAssinanteFactory factory)
        {
            _repository = repository;
            _factory = factory;
        }

        public async Task<AssinanteResponseDTO> CriarAsync(AssinanteCreateDTO dto)
        {
            bool emailEmUso = await _repository.EmailExisteAsync(dto.Email);

            if (emailEmUso)
                throw new ArgumentException("Este e-mail já está cadastrado no sistema.");

            var assinante = _factory.Criar(dto);

            await _repository.AdicionarAsync(assinante);
            await _repository.SalvarAlteracoesAsync();

            return MapToResponse(assinante);
        }

        public async Task<(int Total, IEnumerable<AssinanteResponseDTO> Assinantes)> ListarTodosAsync(int page, int size)
        {
            var (totalRegistros, assinantes) = await _repository.ListarAtivosAsync(page, size);

            return (
                totalRegistros,
                assinantes.Select(MapToResponse)
            );
        }

        public async Task<AssinanteResponseDTO> ObterPorIdAsync(Guid id)
        {
            var assinante = await _repository.ObterAtivoPorIdAsync(id);

            if (assinante == null)
                throw new KeyNotFoundException("Assinante não encontrado ou inativo.");

            return MapToResponse(assinante);
        }

        public async Task AtualizarParcialAsync(Guid id, AssinantePatchDTO dto)
        {
            var assinante = await _repository.ObterPorIdAsync(id);

            if (assinante == null || assinante.Status != Status.Ativo)
                throw new KeyNotFoundException("Assinante ativo não encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.NomeCompleto))
                assinante.SetNome(dto.NomeCompleto);

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                bool emailEmUso = await _repository.EmailExisteAsync(dto.Email, id);

                if (emailEmUso)
                    throw new ArgumentException("E-mail já está em uso.");

                assinante.SetEmail(dto.Email);
            }

            if (dto.Plano.HasValue)
                assinante.SetPlano(dto.Plano.Value);

            if (dto.ValorMensal.HasValue)
                assinante.SetValorMensal(dto.ValorMensal.Value);

            if (dto.Status.HasValue)
            {
                if (dto.Status == Status.Ativo)
                    assinante.Ativar();
                else
                    assinante.Desativar();
            }

            if (dto.DataInicioAssinatura.HasValue)
                assinante.SetDataInicio(dto.DataInicioAssinatura.Value);

            await _repository.SalvarAlteracoesAsync();
        }

        public async Task DesativarAsync(Guid id)
        {
            var assinante = await _repository.ObterPorIdAsync(id);

            if (assinante == null)
                throw new KeyNotFoundException("Assinante não encontrado.");

            assinante.Desativar();

            await _repository.SalvarAlteracoesAsync();
        }

        public async Task DeletarAsync(Guid id)
        {
            var assinante = await _repository.ObterPorIdAsync(id);

            if (assinante == null)
                throw new KeyNotFoundException("Assinante não encontrado.");

            await _repository.RemoverAsync(assinante);
            await _repository.SalvarAlteracoesAsync();
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