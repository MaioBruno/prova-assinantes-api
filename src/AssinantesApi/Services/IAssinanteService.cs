using AssinantesApi.DTOs;

namespace AssinantesApi.Services
{
    public interface IAssinanteService
    {
        Task<AssinanteResponseDTO> CriarAsync(AssinanteCreateDTO dto);

        Task<(int Total, IEnumerable<AssinanteResponseDTO> Assinantes)> ListarTodosAsync(int page, int size);

        Task<AssinanteResponseDTO?> ObterPorIdAsync(Guid id);

        Task AtualizarParcialAsync(Guid id, AssinantePatchDTO dto);

        Task DesativarAsync(Guid id);

        Task DeletarAsync(Guid id);
    }
}