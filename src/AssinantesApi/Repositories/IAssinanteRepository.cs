using AssinantesApi.Entities;

namespace AssinantesApi.Repositories
{
    public interface IAssinanteRepository
    {
        Task<bool> EmailExisteAsync(string email);
        Task<bool> EmailExisteAsync(string email, Guid ignorarId);
        Task AdicionarAsync(Assinante assinante);
        Task<Assinante?> ObterPorIdAsync(Guid id);
        Task<Assinante?> ObterAtivoPorIdAsync(Guid id);
        Task<(int Total, IEnumerable<Assinante> Assinantes)> ListarAtivosAsync(int page, int size);
        Task RemoverAsync(Assinante assinante);
        Task SalvarAlteracoesAsync();
    }
}