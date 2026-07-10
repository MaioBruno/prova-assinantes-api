using AssinantesApi.Entities;

namespace AssinantesApi.DTOs
{
    // Criar os campos obrigatórios iniciais
    public class AssinanteCreateDTO
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataInicioAssinatura { get; set; }
        public Plano Plano { get; set; }
        public decimal ValorMensal { get; set; }
    }

    // Atualização Parcial: campos opcionais (nullable)
    public class AssinantePatchDTO
    {
        public string? NomeCompleto { get; set; }
        public string? Email { get; set; }
        public Plano? Plano { get; set; }
        public decimal? ValorMensal { get; set; }
        public Status? Status { get; set; }
        public DateTime? DataInicioAssinatura { get; set; }
    }

    // Exibe os dados para o usuário, incluindo o cálculo de tempo
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