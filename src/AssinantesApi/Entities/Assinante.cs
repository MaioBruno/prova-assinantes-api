using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace AssinantesApi.Entities
{
    public enum Plano 
    { 
        Basico = 1, 
        Padrao = 2, 
        Premium = 3 
    }
    
    public enum Status 
    { 
        Ativo = 1, 
        Inativo = 2 
    }

    public class Assinante
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de início é obrigatória.")]
        public DateTime DataInicioAssinatura { get; set; }

        [Required]
        public Plano Plano { get; set; }

        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor mensal deve ser maior que 0.")]
        [DefaultValue(49.90)]
        public decimal ValorMensal { get; set; }

        public Status Status { get; set; } = Status.Ativo;

        [NotMapped]
        [JsonIgnore]
        public int TempoDeAssinaturaEmMeses
        {
            get
            {
                var dataAtual = DateTime.UtcNow;
                var meses = ((dataAtual.Year - DataInicioAssinatura.Year) * 12) 
                          + dataAtual.Month - DataInicioAssinatura.Month;

                return meses <= 0 ? 1 : meses;
            }
        }
    }
}