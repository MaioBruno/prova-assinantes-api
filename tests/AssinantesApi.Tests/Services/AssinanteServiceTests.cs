using AssinantesApi.Data;
using AssinantesApi.DTOs;
using AssinantesApi.Entities;
using AssinantesApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AssinantesApi.Tests.Services
{
    public class AssinanteServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoDataForFutura()
        {
            // Arrange
            var context = GetDbContext();
            var service = new AssinanteService(context);
            var dto = new AssinanteCreateDTO
            {
                NomeCompleto = "Teste Data Futura",
                Email = "futuro@teste.com",
                DataInicioAssinatura = DateTime.UtcNow.AddDays(10), 
                Plano = Plano.Padrao,
                ValorMensal = 50
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(dto));
            
            Assert.Equal("A data de início da assinatura não pode ser maior que a data atual.", exception.Message);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoEmailJaExistir()
        {
            // Arrange
            var context = GetDbContext();
            
            context.Assinantes.Add(new Assinante
            {
                NomeCompleto = "Usuário Existente",
                Email = "duplicado@teste.com", 
                DataInicioAssinatura = DateTime.UtcNow.AddMonths(-1),
                Plano = Plano.Basico,
                ValorMensal = 30
            });
            await context.SaveChangesAsync();

            var service = new AssinanteService(context);
            var dto = new AssinanteCreateDTO
            {
                NomeCompleto = "Novo Usuário Tentando o Mesmo Email",
                Email = "duplicado@teste.com", 
                DataInicioAssinatura = DateTime.UtcNow,
                Plano = Plano.Premium,
                ValorMensal = 100
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(dto));
            
            Assert.Equal("Este e-mail já está cadastrado no sistema.", exception.Message);
        }
    }
}