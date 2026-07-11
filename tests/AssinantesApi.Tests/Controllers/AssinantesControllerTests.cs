using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace AssinantesApi.Tests.Controllers
{
    public class AssinantesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AssinantesControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ListarTodos_DeveRetornarSucesso()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/assinantes");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_CriarAssinante_RetornaBadRequest_QuandoDadosInvalidos()
        {
            // Arrange
            var client = _factory.CreateClient();
            var content = new StringContent("{\"nomeCompleto\": \"\"}", System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/assinantes", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}