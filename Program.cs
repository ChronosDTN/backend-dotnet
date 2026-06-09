using ChronosDotnetApi.Infrastructure;
using ChronosDotnetApi.Services; // ADICIONADO
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os servicos necessarios para o funcionamento dos controladores de API REST.
builder.Services.AddControllers();

// Adiciona os geradores de metadados do Swagger para documentacao.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new() {
        Title = "Chronos DTN API",
        Version = "v1",
        Description = "API REST para gerenciamento de nós e topologia da rede DTN Terra-Lua"
    });
});

// Registra a classe ChronosDbContext no container de Injecao de Dependencias.
builder.Services.AddDbContext<ChronosDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// ADICIONADO: Registra a camada de serviço (Clean Architecture)
builder.Services.AddScoped<INodeService, NodeService>();

var app = builder.Build();

// Configura o pipeline de atendimento de requisições HTTP para ambiente local ou desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// REMOVIDO: app.UseAuthorization(); (não há esquema de autenticação configurado)

app.MapControllers();

app.Run();