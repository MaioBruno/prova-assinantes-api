namespace AssinantesApi.DTOs
{
    public class AssinanteCreateDTO
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Plano { get; set; }
        public decimal ValorMensal { get; set; }
        public int Status { get; set; }
    }

    public class AssinanteUpdateDTO
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Plano { get; set; }
        public decimal ValorMensal { get; set; }
        public int Status { get; set; }
    }

    public class AssinantePatchDTO
    {
        public string? NomeCompleto { get; set; }
        public string? Email { get; set; }
        public int? Plano { get; set; }
        public decimal? ValorMensal { get; set; }
        public int? Status { get; set; }
    }

    public class AssinanteResponseDTO
    {
        public Guid Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataInicioAssinatura { get; set; }
        public int Plano { get; set; }
        public decimal ValorMensal { get; set; }
        public int Status { get; set; }
    }
}