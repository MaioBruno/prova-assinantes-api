# 📌 API de Gerenciamento de Assinantes

API REST desenvolvida em .NET para gerenciamento de assinantes, permitindo cadastro, consulta, atualização, desativação e exclusão de registros.

---

## 🚀 Tecnologias utilizadas

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- xUnit & InMemoryDatabase (Testes Unitarios e Integração)
- Swagger (Swashbuckle)
- Data Annotations (validações)

---

## 📁 Estrutura do Projeto
```text
AssinantesApi/
├── Controllers/
├── Services/
├── Data/
├── Entities/
├── DTOs/
├── Migrations/
└── Program.cs

---

## ⚙️ Como executar o projeto

1. Clonar o repositório
git clone https://github.com/seu-repositorio/assinantes-api.git

2. Configurar conexão no appsettings.json

"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR;Database=AssinantesDb;Trusted_Connection=True;TrustServerCertificate=True;"
}

3. Rodar migrations
dotnet ef database update --project src/AssinantesApi

4. Executar aplicação
dotnet run --project src/AssinantesApi

5. Executar os testes unitarios 
dotnet test tests/AssinantesApi.Tests

6. Acessar Swagger
https://localhost:xxxx/swagger

---

## 📌 Funcionalidades

- Criar assinante (validação de email único e restrição de data futura)
- Listar assinantes ativos (com paginação)
- Buscar por ID
- Atualizar parcialmente (PATCH)
- Desativar assinante (Soft delete: altera status para inativo e zera o valor mensal)
- Deletar assinante 

---

## 📊 Regras de Negócio

- Email não pode ser duplicado
- Data de início não pode ser maior que a data atual
- A listagem exibe apenas assinantes ativos
- O tempo de assinatura (em meses) é calculado dinamicamente em tempo de execução
- O valor mensal deve ser maior que zero
- Ao desativar:
  - Status = Inativo
  - ValorMensal = 0

---

## 📦 DTOs

Criação:
{
  "nomeCompleto": "string",
  "email": "string",
  "dataInicioAssinatura": "2024-01-01",
  "plano": 1,
  "valorMensal": 100.00
}

Resposta:
{
  "id": "guid",
  "nomeCompleto": "string",
  "email": "string",
  "dataInicioAssinatura": "2024-01-01",
  "plano": 1,
  "valorMensal": 100.00,
  "status": 1,
  "tempoDeAssinaturaEmMeses": 12
}

Patch:
{
  "nomeCompleto": "string",
  "email": "string",
  "plano": 1,
  "valorMensal": 50.00,
  "status": 1
}

Resposta:
{
  "id": "guid",
  "nomeCompleto": "string",
  "email": "string",
  "dataInicioAssinatura": "2024-01-01",
  "plano": 1,
  "valorMensal": 50.00,
  "status": 1,
  "tempoDeAssinaturaEmMeses": 12
}

---

## 🔢 Enumerações

Status:
- Ativo = 1
- Inativo = 2

Plano: 
- Basico = 1
- Padrao = 2
- Premium = 3
---

## 🧠 Decisões Técnicas

- Uso de DTOs: Implementados para evitar a exposição direta das entidades do banco de dados na API, garantindo maior segurança e controle sobre o tráfego de dados de entrada e saída
- Separação em Camadas: A lógica de negócio foi isolada em uma camada de Serviços (AssinanteService), removendo a responsabilidade dos Controllers. Isso facilita a manutenção, promove o princípio da Responsabilidade Única (SOLID) e viabiliza testes unitários isolados
- Entity Framework Core: Escolhido como ORM pela sua robustez, integração nativa com o ecossistema .NET e facilidade na gestão de Migrations
- Validações com Data Annotations: Utilizadas para garantir a integridade dos dados logo na entrada da requisição, devolvendo retornos HTTP 400 consistentes sem precisar sujar a lógica de negócio.
- Paginação de Dados: Implementada no endpoint de listagem para garantir performance e escalabilidade, evitando sobrecarga de tráfego e memória ao retornar muitos registros.
- Domínio Rico (Princípios de DDD): As lógicas intrínsecas à entidade, como o cálculo dinâmico de meses de assinatura e inicialização de IDs, foram encapsuladas no próprio modelo 'Assinante', evitando o antipadrão de modelo anêmico
- Testes Automatizados: Implementação da pirâmide de testes cobrindo as regras das Entidades, a lógica do Service (com banco em memória) e o comportamento do Controller (Testes de Integração)

---

## 🛠️ Melhorias Futuras

- Implementação de Autenticação/Autorização via JWT [Para ter mais segurança e confiabilidade no sistema]
- Criação de um Middleware global para tratamento de exceções (Error Handling) [Para facilitiar os Controllers tirando try/catch. Além  de identifica erros]
- Implementação de Cache (Redis) para a listagem de assinantes [Para melhorar o tempo de resposta e poupando o banco de dados]

---

## 👨‍💻 Autor

Bruno Maio 
(https://github.com/MaioBruno)
