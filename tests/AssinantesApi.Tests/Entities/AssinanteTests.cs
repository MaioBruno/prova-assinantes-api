using AssinantesApi.Entities;
using Xunit;

namespace AssinantesApi.Tests.Entities
{
    public class AssinanteTests
    {
        [Fact]
        public void TempoDeAssinaturaEmMeses_DeveCalcularCorretamente_QuandoPassaremMeses()
        {
            // Arrange
            var dataInicio = DateTime.UtcNow.AddMonths(-5); 
            
            var assinante = new Assinante 
            { 
                NomeCompleto = "João Teste",
                Email = "joao@teste.com",
                DataInicioAssinatura = dataInicio,
                Plano = Plano.Basico
            };

            // Act
            var mesesCalculados = assinante.TempoDeAssinaturaEmMeses;

            // Assert
            Assert.Equal(5, mesesCalculados);
        }

        [Fact]
        public void TempoDeAssinaturaEmMeses_DeveRetornarMinimoUm_QuandoAssinaturaForCriadaHoje()
        {
            // Arrange
            var assinante = new Assinante 
            { 
                NomeCompleto = "Thayna Teste",
                Email = "thayna@teste.com",
                DataInicioAssinatura = DateTime.UtcNow,
                Plano = Plano.Premium
            };

            // Act
            var mesesCalculados = assinante.TempoDeAssinaturaEmMeses;

            // Assert
            Assert.Equal(1, mesesCalculados);
        }
    }
}