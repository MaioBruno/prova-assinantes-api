using System;

namespace AssinantesApi.Entities
{
    public enum Plano { Basico, Padrao, Premium }
    public enum Status { Inativo, Ativo }

    public class Assinante
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataInicioAssinatura { get; set; }
        public Plano Plano { get; set; }
        public decimal ValorMensal { get; set; }
        public Status Status { get; set; } = Status.Ativo;

        // Regra de negócio: calculado dinamicamente, não persistido
        public int TempoDeAssinaturaEmMeses
        {
            get
            {
                var diff = DateTime.Now - DataInicioAssinatura;
                int meses = (int)(diff.TotalDays / 30.44);
                return meses <= 0 ? 1 : meses; // Regra: não pode ser 0
            }
        }
    }
}