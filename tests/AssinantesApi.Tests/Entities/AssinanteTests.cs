using System;
using Xunit;
using AssinantesApi.Entities;
using AssinantesApi.Enums;

namespace AssinantesApi.Tests.Entities
{
    public class AssinanteTests
    {
        private static Assinante CriarAssinanteValido()
        {
            return new Assinante(
                "João Silva",
                "joao@email.com",
                DateTime.UtcNow.AddDays(-40),
                Plano.Basico,
                99.90m
            );
        }

        [Fact]
        public void Construtor_DeveCriarAssinante_QuandoDadosForemValidos()
        {
            var assinante = CriarAssinanteValido();

            Assert.NotEqual(Guid.Empty, assinante.Id);
            Assert.Equal("João Silva", assinante.NomeCompleto);
            Assert.Equal("joao@email.com", assinante.Email);
            Assert.Equal(DateTime.UtcNow.Date.AddDays(-40), assinante.DataInicioAssinatura.Date);
            Assert.Equal(Plano.Basico, assinante.Plano);
            Assert.Equal(99.90m, assinante.ValorMensal);
            Assert.Equal(Status.Ativo, assinante.Status);
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoNomeForVazio()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Assinante(
                    "",
                    "joao@email.com",
                    DateTime.UtcNow.AddDays(-40),
                    Plano.Basico,
                    99.90m
                ));

            Assert.Equal("Nome completo é obrigatório.", ex.Message);
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoEmailForInvalido()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Assinante(
                    "João Silva",
                    "email-invalido",
                    DateTime.UtcNow.AddDays(-40),
                    Plano.Basico,
                    99.90m
                ));

            Assert.Equal("Email inválido.", ex.Message);
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoDataForFutura()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Assinante(
                    "João Silva",
                    "joao@email.com",
                    DateTime.UtcNow.AddDays(1),
                    Plano.Basico,
                    99.90m
                ));

            Assert.Equal("A data não pode ser futura.", ex.Message);
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoTempoDeAssinaturaForMenorOuIgualAZero()
        { 
            var ex = Assert.Throws<ArgumentException>(() =>
                new Assinante(
                    "João Silva",
                    "joao@email.com",
                    DateTime.UtcNow.AddDays(-1),
                    Plano.Basico,
                    99.90m
                ));

            Assert.Equal("O tempo de assinatura deve ser maior que zero.", ex.Message);
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoValorMensalForZeroOuNegativo()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Assinante(
                    "João Silva",
                    "joao@email.com",
                    DateTime.UtcNow.AddDays(-40),
                    Plano.Basico,
                    0m
                ));

            Assert.Equal("O valor mensal deve ser maior que zero.", ex.Message);
        }

        [Fact]
        public void SetNome_DeveAtualizarNome_QuandoNomeForValido()
        {
            var assinante = CriarAssinanteValido();

            assinante.SetNome("Maria Silva");

            Assert.Equal("Maria Silva", assinante.NomeCompleto);
        }

        [Fact]
        public void SetNome_DeveLancarExcecao_QuandoNomeForVazio()
        {
            var assinante = CriarAssinanteValido();

            var ex = Assert.Throws<ArgumentException>(() => assinante.SetNome(""));

            Assert.Equal("Nome completo é obrigatório.", ex.Message);
        }

        [Fact]
        public void SetEmail_DeveAtualizarEmail_QuandoEmailForValido()
        {
            var assinante = CriarAssinanteValido();

            assinante.SetEmail("novo@email.com");

            Assert.Equal("novo@email.com", assinante.Email);
        }

        [Fact]
        public void SetEmail_DeveLancarExcecao_QuandoEmailForInvalido()
        {
            var assinante = CriarAssinanteValido();

            var ex = Assert.Throws<ArgumentException>(() => assinante.SetEmail("email-invalido"));

            Assert.Equal("Email inválido.", ex.Message);
        }

        [Fact]
        public void SetDataInicio_DeveAtualizarData_QuandoDataForValida()
        {
            var assinante = CriarAssinanteValido();
            var novaData = DateTime.UtcNow.AddDays(-60);

            assinante.SetDataInicio(novaData);

            Assert.Equal(novaData.Date, assinante.DataInicioAssinatura.Date);
        }

        [Fact]
        public void SetDataInicio_DeveLancarExcecao_QuandoDataForFutura()
        {
            var assinante = CriarAssinanteValido();

            var ex = Assert.Throws<ArgumentException>(() =>
                assinante.SetDataInicio(DateTime.UtcNow.AddDays(1)));

            Assert.Equal("A data não pode ser futura.", ex.Message);
        }

        [Fact]
        public void SetDataInicio_DeveLancarExcecao_QuandoTempoDeAssinaturaForMenorOuIgualAZero()
        {
            var assinante = CriarAssinanteValido();

            var ex = Assert.Throws<ArgumentException>(() =>
                assinante.SetDataInicio(DateTime.UtcNow.AddDays(-1)));

            Assert.Equal("O tempo de assinatura deve ser maior que zero.", ex.Message);
        }

        [Fact]
        public void SetPlano_DeveAtualizarPlano_QuandoPlanoForValido()
        {
            var assinante = CriarAssinanteValido();

            assinante.SetPlano(Plano.Premium);

            Assert.Equal(Plano.Premium, assinante.Plano);
        }

        [Fact]
        public void SetPlano_DeveLancarExcecao_QuandoPlanoForInvalido()
        {
            var assinante = CriarAssinanteValido();

            var ex = Assert.Throws<ArgumentException>(() => assinante.SetPlano((Plano)999));

            Assert.Equal("Plano inválido.", ex.Message);
        }

        [Fact]
        public void SetValorMensal_DeveAtualizarValor_QuandoValorForValido()
        {
            var assinante = CriarAssinanteValido();

            assinante.SetValorMensal(150m);

            Assert.Equal(150m, assinante.ValorMensal);
        }

        [Fact]
        public void SetValorMensal_DeveLancarExcecao_QuandoValorForZeroOuNegativo()
        {
            var assinante = CriarAssinanteValido();

            var ex = Assert.Throws<ArgumentException>(() => assinante.SetValorMensal(0));

            Assert.Equal("O valor mensal deve ser maior que zero.", ex.Message);
        }

        [Fact]
        public void Desativar_DeveAlterarStatusEZerarValorMensal()
        {
            var assinante = CriarAssinanteValido();

            assinante.Desativar();

            Assert.Equal(Status.Inativo, assinante.Status);
            Assert.Equal(0m, assinante.ValorMensal);
        }

        [Fact]
        public void Desativar_DeveLancarExcecao_QuandoJaEstiverInativo()
        {
            var assinante = CriarAssinanteValido();
            assinante.Desativar();

            var ex = Assert.Throws<InvalidOperationException>(() => assinante.Desativar());

            Assert.Equal("Assinante já está inativo.", ex.Message);
        }

        [Fact]
        public void Ativar_DeveAlterarStatusParaAtivo()
        {
            var assinante = CriarAssinanteValido();
            assinante.Desativar();

            assinante.Ativar();

            Assert.Equal(Status.Ativo, assinante.Status);
        }

        [Fact]
        public void Ativar_DeveLancarExcecao_QuandoJaEstiverAtivo()
        {
            var assinante = CriarAssinanteValido();

            var ex = Assert.Throws<InvalidOperationException>(() => assinante.Ativar());

            Assert.Equal("Assinante já está ativo.", ex.Message);
        }

        [Fact]
        public void TempoDeAssinaturaEmMeses_DeveRetornarValorNaoNegativo()
        {
            var assinante = CriarAssinanteValido();

            Assert.True(assinante.TempoDeAssinaturaEmMeses >= 0);
        }
    }
}