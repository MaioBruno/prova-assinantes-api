using AssinantesApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssinantesApi.DTOs
{
    public class AssinanteCreateDTO
    {
        [Required(ErrorMessage = "Nome completo é obrigatório.")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de início é obrigatória.")]
        public DateTime DataInicioAssinatura { get; set; }

        [Required(ErrorMessage = "Plano é obrigatório.")]
        public Plano Plano { get; set; }

        [Required(ErrorMessage = "Valor mensal é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor mensal deve ser maior que zero.")]
        public decimal ValorMensal { get; set; }
    }

    public class AssinantePatchDTO
    {
        public string? NomeCompleto { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string? Email { get; set; }

        public Plano? Plano { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor mensal deve ser maior que zero.")]
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