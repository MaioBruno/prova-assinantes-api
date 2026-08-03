using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AssinantesApi.DTOs;
using AssinantesApi.Entities;
using AssinantesApi.Enums;
using AssinantesApi.Factories;
using AssinantesApi.Repositories;
using AssinantesApi.Services;

namespace AssinantesApi.Tests.Services
{
    public class AssinanteServiceTests
    {
        private readonly Mock<IAssinanteRepository> _repositoryMock;
        private readonly Mock<IAssinanteFactory> _factoryMock;
        private readonly AssinanteService _service;

        public AssinanteServiceTests()
        {
            _repositoryMock = new Mock<IAssinanteRepository>();
            _factoryMock = new Mock<IAssinanteFactory>();
            _service = new AssinanteService(_repositoryMock.Object, _factoryMock.Object);
        }

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

        private static AssinanteCreateDTO CriarDtoValido()
        {
            return new AssinanteCreateDTO
            {
                NomeCompleto = "João Silva",
                Email = "joao@email.com",
                DataInicioAssinatura = DateTime.UtcNow.AddDays(-40),
                Plano = Plano.Basico,
                ValorMensal = 99.90m
            };
        }

        [Fact]
        public async Task CriarAsync_DeveCriarAssinante_QuandoDadosForemValidos()
        {
            var dto = CriarDtoValido();
            var assinante = CriarAssinanteValido();

            _repositoryMock.Setup(r => r.EmailExisteAsync(dto.Email))
                .ReturnsAsync(false);

            _factoryMock.Setup(f => f.Criar(dto))
                .Returns(assinante);

            var resultado = await _service.CriarAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(assinante.Id, resultado.Id);
            Assert.Equal(dto.NomeCompleto, resultado.NomeCompleto);
            Assert.Equal(dto.Email, resultado.Email);

            _repositoryMock.Verify(r => r.EmailExisteAsync(dto.Email), Times.Once);
            _factoryMock.Verify(f => f.Criar(dto), Times.Once);
            _repositoryMock.Verify(r => r.AdicionarAsync(assinante), Times.Once);
            _repositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoEmailJaEstiverEmUso()
        {
            var dto = CriarDtoValido();

            _repositoryMock.Setup(r => r.EmailExisteAsync(dto.Email))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));

            Assert.Equal("Este e-mail já está cadastrado no sistema.", ex.Message);

            _repositoryMock.Verify(r => r.EmailExisteAsync(dto.Email), Times.Once);
            _factoryMock.Verify(f => f.Criar(It.IsAny<AssinanteCreateDTO>()), Times.Never);
            _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Assinante>()), Times.Never);
            _repositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
        }

        [Fact]
        public async Task ListarTodosAsync_DeveRetornarSomenteAtivos()
        {
            var assinante1 = CriarAssinanteValido();
            var assinante2 = new Assinante("Maria Silva", "maria@email.com", DateTime.UtcNow.AddDays(-50), Plano.Premium, 150m);

            var lista = new List<Assinante> { assinante1, assinante2 };

            _repositoryMock.Setup(r => r.ListarAtivosAsync(1, 10))
                .ReturnsAsync((2, lista));

            var resultado = await _service.ListarTodosAsync(1, 10);

            Assert.Equal(2, resultado.Total);
            Assert.Equal(2, resultado.Assinantes.Count());
            Assert.All(resultado.Assinantes, a => Assert.Equal(Status.Ativo, a.Status));
            _repositoryMock.Verify(r => r.ListarAtivosAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarAssinante_QuandoEncontradoEAtivo()
        {
            var assinante = CriarAssinanteValido();

            _repositoryMock.Setup(r => r.ObterAtivoPorIdAsync(assinante.Id))
                .ReturnsAsync(assinante);

            var resultado = await _service.ObterPorIdAsync(assinante.Id);

            Assert.NotNull(resultado);
            Assert.Equal(assinante.Id, resultado.Id);
            Assert.Equal(assinante.Email, resultado.Email);

            _repositoryMock.Verify(r => r.ObterAtivoPorIdAsync(assinante.Id), Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveLancarExcecao_QuandoNaoEncontradoOuInativo()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.ObterAtivoPorIdAsync(id))
                .ReturnsAsync((Assinante?)null);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ObterPorIdAsync(id));

            Assert.Equal("Assinante não encontrado ou inativo.", ex.Message);
            _repositoryMock.Verify(r => r.ObterAtivoPorIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task AtualizarParcialAsync_DeveAtualizarCampos_QuandoAssinanteAtivoEDtoValido()
        {
            var assinante = CriarAssinanteValido();

            var dto = new AssinantePatchDTO
            {
                NomeCompleto = "Novo Nome",
                Email = "novo@email.com",
                Plano = Plano.Premium,
                ValorMensal = 199.90m,
                DataInicioAssinatura = DateTime.UtcNow.AddDays(-60)
            };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id))
                .ReturnsAsync(assinante);

            _repositoryMock.Setup(r => r.EmailExisteAsync(dto.Email, assinante.Id))
                .ReturnsAsync(false);

            await _service.AtualizarParcialAsync(assinante.Id, dto);

            Assert.Equal("Novo Nome", assinante.NomeCompleto);
            Assert.Equal("novo@email.com", assinante.Email);
            Assert.Equal(Plano.Premium, assinante.Plano);
            Assert.Equal(199.90m, assinante.ValorMensal);
            Assert.Equal(Status.Ativo, assinante.Status);

            _repositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        [Fact]
        public async Task AtualizarParcialAsync_DeveLancarExcecao_QuandoAssinanteNaoForAtivoOuNaoExistir()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((Assinante?)null);

            var dto = new AssinantePatchDTO { NomeCompleto = "Novo Nome" };

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AtualizarParcialAsync(id, dto));

            Assert.Equal("Assinante ativo não encontrado.", ex.Message);
        }

        [Fact]
        public async Task AtualizarParcialAsync_DeveLancarExcecao_QuandoEmailJaEstiverEmUso()
        {
            var assinante = CriarAssinanteValido();

            var dto = new AssinantePatchDTO
            {
                Email = "jaexiste@email.com"
            };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id))
                .ReturnsAsync(assinante);

            _repositoryMock.Setup(r => r.EmailExisteAsync(dto.Email, assinante.Id))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarParcialAsync(assinante.Id, dto));

            Assert.Equal("E-mail já está em uso.", ex.Message);
        }

        [Fact]
        public async Task DesativarAsync_DeveDesativarAssinante_QuandoExistir()
        {
            var assinante = CriarAssinanteValido();

            _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id))
                .ReturnsAsync(assinante);

            await _service.DesativarAsync(assinante.Id);

            Assert.Equal(Status.Inativo, assinante.Status);
            Assert.Equal(0m, assinante.ValorMensal);

            _repositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        [Fact]
        public async Task DesativarAsync_DeveLancarExcecao_QuandoAssinanteNaoExistir()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((Assinante?)null);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DesativarAsync(id));

            Assert.Equal("Assinante não encontrado.", ex.Message);
        }

        [Fact]
        public async Task DeletarAsync_DeveRemoverAssinante_QuandoExistir()
        {
            var assinante = CriarAssinanteValido();

            _repositoryMock.Setup(r => r.ObterPorIdAsync(assinante.Id))
                .ReturnsAsync(assinante);

            await _service.DeletarAsync(assinante.Id);

            _repositoryMock.Verify(r => r.RemoverAsync(assinante), Times.Once);
            _repositoryMock.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeletarAsync_DeveLancarExcecao_QuandoAssinanteNaoExistir()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((Assinante?)null);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeletarAsync(id));

            Assert.Equal("Assinante não encontrado.", ex.Message);
        }
    }
}