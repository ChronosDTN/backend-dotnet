using ChronosDotnetApi.Infrastructure;
using ChronosDotnetApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new() {
        Title = "Chronos DTN API",
        Version = "v1",
        Description = "API REST para gerenciamento de nós e topologia da rede DTN Terra-Lua"
    });
});

builder.Services.AddDbContext<ChronosDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INodeService, NodeService>();
builder.Services.AddScoped<IAssetBalanceService, AssetBalanceService>();
builder.Services.AddScoped<INodeLinkService, NodeLinkService>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.MapControllers();

app.Run();