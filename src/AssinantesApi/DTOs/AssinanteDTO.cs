using AssinantesApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace AssinantesApi.DTOs
{
    public class AssinanteCreateDTO
    {
        [Required]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime DataInicioAssinatura { get; set; }

        [Required]
        public Plano Plano { get; set; }

        [Required]
        public decimal ValorMensal { get; set; }
    }

    public class AssinantePatchDTO
    {
        public string? NomeCompleto { get; set; }
        public string? Email { get; set; }
        public Plano? Plano { get; set; }
        public decimal? ValorMensal { get; set; }
        public Status? Status { get; set; }
        public DateTime? DataInicioAssinatura { get; set; }
    }

    public class AssinanteResponseDTO
    {
        public Guid Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataInicioAssinatura { get; set; }
        public Plano Plano { get; set; }
        public decimal ValorMensal { get; set; }
        public Status Status { get; set; }
        public int TempoDeAssinaturaEmMeses { get; set; }
    }
}