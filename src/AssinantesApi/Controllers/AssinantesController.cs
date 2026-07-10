using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssinantesApi.Data;
using AssinantesApi.Entities;

namespace AssinantesApi.Controllers
{
    [ApiController]
    [Route("assinantes")]
    public class AssinantesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssinantesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost] // POST /assinantes
        public async Task<IActionResult> Criar([FromBody] Assinante assinante)
        {
            if (assinante == null) return BadRequest("Dados do assinante inválidos.");

            if (assinante.DataInicioAssinatura > DateTime.UtcNow)
                return BadRequest("A data de início da assinatura não pode ser maior que a data atual.");

            bool emailEmUso = await _context.Assinantes.AnyAsync(a => a.Email == assinante.Email);
            if (emailEmUso) return BadRequest("Este e-mail já está cadastrado no sistema.");

            assinante.Id = Guid.NewGuid();
            _context.Assinantes.Add(assinante);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = assinante.Id }, assinante);
        }

        [HttpGet] // GET /assinantes
        public async Task<IActionResult> ListarTodos(int page = 1, int size = 10)
        {
            var query = _context.Assinantes.Where(a => a.Status == Status.Ativo);
            // Conta o total antes de aplicar Skip/Take
            int totalRegistros = await query.CountAsync();

            // Validação simples para evitar valores negativos
            if (page < 1) page = 1;
            if (size < 1) size = 10;

            var assinantes = await _context.Assinantes
                .Where(a => a.Status == Status.Ativo)
                .OrderBy(a => a.NomeCompleto) 
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            // Adiciona o total no cabeçalho da resposta
            Response.Headers.Append("X-Total-Count", totalRegistros.ToString());

            return Ok(assinantes);
        }

        [HttpGet("{id:guid}")] // GET /assinantes/:id
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var assinante = await _context.Assinantes
                .FirstOrDefaultAsync(a => a.Id == id && a.Status == Status.Ativo);

            if (assinante == null) return NotFound($"Assinante ativo com ID {id} não encontrado.");

            return Ok(assinante);
        }

        [HttpPatch("{id:guid}")] // PATCH /assinantes/:id
        public async Task<IActionResult> UpdateParcial(Guid id, [FromBody] Assinante patchData)
        {
            var assinante = await _context.Assinantes.FindAsync(id);
            
            if (assinante == null || assinante.Status != Status.Ativo) 
                return NotFound("Assinante ativo não encontrado.");

            // Atualiza apenas o que foi preenchido no JSON
            if (!string.IsNullOrWhiteSpace(patchData.NomeCompleto)) 
                assinante.NomeCompleto = patchData.NomeCompleto;
                
            if (!string.IsNullOrWhiteSpace(patchData.Email)) 
            {
                bool emailEmUso = await _context.Assinantes.AnyAsync(a => a.Email == patchData.Email && a.Id != id);
                if (emailEmUso) return BadRequest("Este e-mail já está em uso.");
                assinante.Email = patchData.Email;
            }

            if (patchData.Plano != 0) 
                assinante.Plano = patchData.Plano;
                
            if (patchData.ValorMensal > 0) 
                assinante.ValorMensal = patchData.ValorMensal;

            if (patchData.Status != 0) 
                assinante.Status = patchData.Status;

            if (patchData.DataInicioAssinatura != default)
            {
                if (patchData.DataInicioAssinatura > DateTime.UtcNow)
                    return BadRequest("Data de início não pode ser futura.");
                assinante.DataInicioAssinatura = patchData.DataInicioAssinatura;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:guid}/desativar")] // PATCH /assinantes/:id/desativar
        public async Task<IActionResult> Desativar(Guid id)
        {
            var assinante = await _context.Assinantes.FindAsync(id);
            if (assinante == null) return NotFound("Assinante não encontrado.");

            assinante.Status = Status.Inativo;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")] // DELETE /assinantes/:id
        public async Task<IActionResult> DeleteAssinante(Guid id)
        {
            var assinante = await _context.Assinantes.FindAsync(id);
            if (assinante == null) return NotFound($"Assinante com ID {id} não encontrado.");

            _context.Assinantes.Remove(assinante);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}