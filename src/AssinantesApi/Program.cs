using Microsoft.EntityFrameworkCore;
using AssinantesApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o suporte aos Controllers que criamos
builder.Services.AddControllers(); 

// Adiciona o motor da interface visual do Swagger
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();           

// Conecta ao Banco de Dados SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configura o painel do Swagger apenas para o ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapeia as rotas dos nossos Controllers
app.MapControllers();

app.Run();