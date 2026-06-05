using ChronosDotnetApi.Infrastructure; // Importa a camada de dados contendo o DbContext.
using Microsoft.EntityFrameworkCore; // Importa componentes de extensao do EF Core.

var builder = WebApplication.CreateBuilder(args); // Cria o construtor da aplicacao Web principal do ASP.NET.

// Adiciona os servicos necessarios para o funcionamento dos controladores de API REST.
builder.Services.AddControllers();

// Adiciona os geradores de metadados do Swagger para documentacao.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra a classe ChronosDbContext no container de Injecao de Dependencias.
builder.Services.AddDbContext<ChronosDbContext>(options =>
    // Configura o Entity Framework Core para utilizar o driver gerenciado oficial do Oracle Database.
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build(); // Constroi a aplicacao com base nos servicos configurados acima.

// Configura o pipeline de atendimento de requisições HTTP para ambiente local ou desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Habilita o middleware de geracao do Swagger JSON.
    app.UseSwaggerUI(); // Habilita o middleware de interface grafica Web do Swagger.
}

app.UseAuthorization(); // Habilita o middleware de autorizacao de rotas de seguranca.

app.MapControllers(); // Realiza o mapeamento automatico de rotas com base nas anotacoes dos Controllers.

app.Run(); // Inicia a escuta por requisições HTTP da API Web.
