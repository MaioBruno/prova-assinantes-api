using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssinantesApi.Data;
using AssinantesApi.Entities;
using AssinantesApi.DTOs;

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
        public async Task<IActionResult> Criar([FromBody] AssinanteCreateDTO dto)
        {
            // 1. Validar se o e-mail já existe (usando o DTO)
            bool emailEmUso = await _context.Assinantes.AnyAsync(a => a.Email == dto.Email);
            if (emailEmUso) return BadRequest("Este e-mail já está cadastrado no sistema.");

            // 2. Validar data (usando o DTO)
            if (dto.DataInicioAssinatura > DateTime.UtcNow)
                return BadRequest("A data de início da assinatura não pode ser maior que a data atual.");

            // 3. Mapear DTO para Entidade
            var assinante = new Assinante {
                Id = Guid.NewGuid(), // Geramos o ID aqui na Entidade, não no DTO
                NomeCompleto = dto.NomeCompleto,
                Email = dto.Email,
                DataInicioAssinatura = dto.DataInicioAssinatura,
                Plano = dto.Plano,
                ValorMensal = dto.ValorMensal,
                Status = Status.Ativo // Definimos o padrão
            };

            // 4. Salvar
            _context.Assinantes.Add(assinante);
            await _context.SaveChangesAsync();

            // 5. Retornar um ResponseDTO (boa prática não expor a entidade direto)
            var response = new AssinanteResponseDTO {
                Id = assinante.Id,
                NomeCompleto = assinante.NomeCompleto,
                Email = assinante.Email,
                DataInicioAssinatura = assinante.DataInicioAssinatura,
                Plano = assinante.Plano,
                ValorMensal = assinante.ValorMensal,
                Status = assinante.Status,
                TempoDeAssinaturaEmMeses = assinante.TempoDeAssinaturaEmMeses
            };

            return CreatedAtAction(nameof(ObterPorId), new { id = assinante.Id }, response);
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

            var assinantes = await query
                .OrderBy(a => a.NomeCompleto)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(a => new AssinanteResponseDTO {
                    Id = a.Id,
                    NomeCompleto = a.NomeCompleto,
                    Email = a.Email,
                    DataInicioAssinatura = a.DataInicioAssinatura,
                    Plano = a.Plano,         
                    ValorMensal = a.ValorMensal,
                    Status = a.Status,       
                    TempoDeAssinaturaEmMeses = a.TempoDeAssinaturaEmMeses
                })
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", totalRegistros.ToString());

            return Ok(new { Total = totalRegistros, Assinantes = assinantes });
        }

        [HttpGet("{id:guid}")] // GET /assinantes/:id
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var a = await _context.Assinantes.FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Ativo);
            if (a == null) return NotFound($"Assinante ativo com ID {id} não encontrado.");

            var response = new AssinanteResponseDTO {
                Id = a.Id,
                NomeCompleto = a.NomeCompleto,
                Email = a.Email,
                DataInicioAssinatura = a.DataInicioAssinatura,
                Plano = a.Plano,
                ValorMensal = a.ValorMensal,
                Status = a.Status,
                TempoDeAssinaturaEmMeses = a.TempoDeAssinaturaEmMeses
            };

            return Ok(response);
        }

        [HttpPatch("{id:guid}")] // PATCH /assinantes/:id
        public async Task<IActionResult> UpdateParcial(Guid id, [FromBody] AssinantePatchDTO dto)
        {
            var a = await _context.Assinantes.FindAsync(id);
            if (a == null || a.Status != Status.Ativo) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.NomeCompleto)) a.NomeCompleto = dto.NomeCompleto;
            
            if (!string.IsNullOrWhiteSpace(dto.Email)) {
                if (await _context.Assinantes.AnyAsync(x => x.Email == dto.Email && x.Id != id))
                    return BadRequest("E-mail já está em uso.");
                a.Email = dto.Email;
            }

            if (dto.Plano.HasValue) a.Plano = dto.Plano.Value;
            if (dto.ValorMensal.HasValue) a.ValorMensal = dto.ValorMensal.Value;
            if (dto.Status.HasValue) a.Status = dto.Status.Value;
            if (dto.DataInicioAssinatura.HasValue) {
                if (dto.DataInicioAssinatura > DateTime.UtcNow) return BadRequest("Data futura.");
                a.DataInicioAssinatura = dto.DataInicioAssinatura.Value;
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