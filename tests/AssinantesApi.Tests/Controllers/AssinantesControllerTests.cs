using Microsoft.AspNetCore.Mvc;
using AssinantesApi.DTOs;
using AssinantesApi.Services;

namespace AssinantesApi.Controllers
{
    [ApiController]
    [Route("assinantes")]
    public class AssinantesController : ControllerBase
    {
        private readonly IAssinanteService _assinanteService;

        public AssinantesController(IAssinanteService assinanteService)
        {
            _assinanteService = assinanteService;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] AssinanteCreateDTO dto)
        {
            var resultado = await _assinanteService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (total, assinantes) = await _assinanteService.ListarTodosAsync(page, size);

            Response.Headers["X-Total-Count"] = total.ToString();

            return Ok(new
            {
                Total = total,
                Page = page,
                Size = size,
                Assinantes = assinantes
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var assinante = await _assinanteService.ObterPorIdAsync(id);
            return Ok(assinante);
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateParcial(Guid id, [FromBody] AssinantePatchDTO dto)
        {
            await _assinanteService.AtualizarParcialAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id:guid}/desativar")]
        public async Task<IActionResult> DesativarAssinante(Guid id)
        {
            await _assinanteService.DesativarAsync(id);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAssinante(Guid id)
        {
            await _assinanteService.DeletarAsync(id);
            return NoContent();
        }
    }
}