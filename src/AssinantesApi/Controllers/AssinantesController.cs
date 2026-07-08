using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssinantesApi.Data;
using AssinantesApi.Entities;

namespace AssinantesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // O endereço será: api/assinantes
    public class AssinantesController : ControllerBase
    {
        private readonly AppDbContext _context;

        // O .NET injeta o nosso contexto do banco automaticamente aqui
        public AssinantesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/assinantes
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Assinante assinante)
        {
            if (assinante == null)
            {
                return BadRequest("Dados do assinante inválidos.");
            }

            // Adiciona o assinante ao contexto e salva no banco de verdade
            _context.Assinantes.Add(assinante);
            await _context.SaveChangesAsync();

            // Retorna o status 201 (Created) e a rota para acessar esse assinante específico
            return CreatedAtAction(nameof(ObterPorId), new { id = assinante.Id }, assinante);
        }

        // GET: api/assinantes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var assinante = await _context.Assinantes.FindAsync(id);

            if (assinante == null)
            {
                return NotFound($"Assinante com ID {id} não encontrado.");
            }

            return Ok(assinante);
        }

        // GET: api/assinantes
        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var assinantes = await _context.Assinantes.ToListAsync();
            return Ok(assinantes);
        }
    }
}