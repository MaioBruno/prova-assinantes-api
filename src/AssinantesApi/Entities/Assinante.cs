using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace AssinantesApi.Entities
{
    // Ao definir números exatos (1, 2, 3), garante que o banco de dados grave exatamente o valor que a regra de negócios pede
    public enum Plano 
    { 
        Basico = 1, Padrao = 2, Premium = 3 
    }
    
    public enum Status 
    { 
        Ativo = 1, Inativo = 2 
    }

    public class Assinante
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // O Required barra a requisição automaticamente se o usuário tentar mandar um JSON sem nome
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string NomeCompleto { get; set; } = string.Empty;

        // O EmailAddress vai validar se a string tem formato de e-mail 
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
        public string Email { get; set; } = string.Empty;

        public DateTime DataInicioAssinatura { get; set; }

        public Plano Plano { get; set; }

        // O Range define um valor mínimo e máximo 
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor mensal da assinatura deve ser maior que 0.")]
        [DefaultValue(49.90)]
        public decimal ValorMensal { get; set; }

        public Status Status { get; set; } = Status.Ativo;

        // O NotMapped avisa ao Entity Framewor para não criar uma coluna disso no SQL Server
        [NotMapped]
        [JsonIgnore] // Para não pedir no swagger 
        public int TempoDeAssinaturaEmMeses
        {
            get
            {
                // UtcNow para evitar falhas de fuso horário do servidor
                var dataAtual = DateTime.UtcNow;
                var meses = ((dataAtual.Year - DataInicioAssinatura.Year) * 12) + dataAtual.Month - DataInicioAssinatura.Month;
                // Se a pessoa assinou no mês atual, a conta daria 0, então forcei a ser 1.
                return meses <= 0 ? 1 : meses; 
            }
        }
    }
}