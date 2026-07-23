using System.Text.RegularExpressions;
using AssinantesApi.Enums;

namespace AssinantesApi.Entities
{
    public class Assinante
    {
        public Guid Id { get; private set; }

        public string NomeCompleto { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public DateTime DataInicioAssinatura { get; private set; }

        public Plano Plano { get; private set; }

        public decimal ValorMensal { get; private set; }

        public Status Status { get; private set; }

        public int TempoDeAssinaturaEmMeses
        {
            get
            {
                var dataAtual = DateTime.UtcNow;

                var meses = ((dataAtual.Year - DataInicioAssinatura.Year) * 12)
                          + dataAtual.Month - DataInicioAssinatura.Month;

                return meses < 0 ? 0 : meses; 
            }
        }

        private Assinante() { }

        public Assinante(
            string nomeCompleto,
            string email,
            DateTime dataInicio,
            Plano plano,
            decimal valorMensal)
        {
            Id = Guid.NewGuid();

            SetNome(nomeCompleto);
            SetEmail(email);
            SetDataInicio(dataInicio);
            SetPlano(plano);
            SetValorMensal(valorMensal);

            Status = Status.Ativo;
        }


        public void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome completo é obrigatório.");

            NomeCompleto = nome;
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório.");

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Email inválido.");

            Email = email;
        }

        public void SetDataInicio(DateTime dataInicio)
        {
            if (dataInicio > DateTime.UtcNow)
                throw new ArgumentException("A data não pode ser futura.");

            DataInicioAssinatura = dataInicio;
        }

        public void SetPlano(Plano plano)
        {
            if (!Enum.IsDefined(typeof(Plano), plano))
                throw new ArgumentException("Plano inválido.");

            Plano = plano;
        }

        public void SetValorMensal(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor mensal deve ser maior que zero.");

            ValorMensal = valor;
        }

        public void Ativar()
        {
            if (Status == Status.Ativo)
                throw new InvalidOperationException("Assinante já está ativo.");

            Status = Status.Ativo;
        }

        public void Desativar()
        {
            if (Status == Status.Inativo)
                throw new InvalidOperationException("Assinante já está inativo.");

            Status = Status.Inativo;
            ValorMensal = 0;
        }
    }
}