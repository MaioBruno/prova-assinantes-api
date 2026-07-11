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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var resultado = await _assinanteService.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (total, assinantes) = await _assinanteService.ListarTodosAsync(page, size);
            
            Response.Headers.Append("X-Total-Count", total.ToString());
            
            return Ok(new { Total = total, Page = page, Size = size, Assinantes = assinantes });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var assinante = await _assinanteService.ObterPorIdAsync(id);
            if (assinante == null)
                return NotFound("Assinante não encontrado ou inativo.");

            return Ok(assinante);
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateParcial(Guid id, [FromBody] AssinantePatchDTO dto)
        {
            try
            {
                await _assinanteService.UpdateParcialAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id:guid}/desativar")]
        public async Task<IActionResult> DesativarAssinante(Guid id)
        {
            try
            {
                await _assinanteService.DesativarAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAssinante(Guid id)
        {
            try
            {
                await _assinanteService.DeletarAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}