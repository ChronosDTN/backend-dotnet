# Chronos DTN — API Secundária (.NET 8)

> Módulo de API REST secundária do gateway financeiro **Chronos DTN**, responsável pelo gerenciamento de nós ativos, saldos de ativos digitais e links de topologia da rede cislunar. Construído em **.NET 8** com **Clean Architecture** e **Entity Framework Core** conectado ao Oracle Database.

---

## 🛰️ Sobre o Módulo

Esta API complementa o backend principal em Java, expondo endpoints REST para operações de leitura e escrita sobre as entidades de infraestrutura da rede DTN (Delay-Tolerant Network) Terra-Lua. Toda a persistência é realizada via **Oracle Database XE 21c** utilizando o driver oficial da Oracle para .NET.

O módulo segue os princípios da **Clean Architecture**, separando claramente as responsabilidades em camadas de domínio, infraestrutura e apresentação.

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Versão | Função |
|---|---|---|
| .NET / ASP.NET Core | 8.0 | Framework principal da API Web |
| Entity Framework Core | 8.x | ORM para mapeamento objeto-relacional |
| Oracle.EntityFrameworkCore | 8.x | Driver oficial Oracle para EF Core |
| Swagger / OpenAPI | Swashbuckle | Documentação interativa dos endpoints |
| Oracle Database XE | 21c | Banco de dados relacional de persistência |

---

### ⚙️ Configuração de Secrets

**IMPORTANTE:** Nunca commite senhas reais no Git!

1. Crie um arquivo `appsettings.Development.json` (já está no .gitignore):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=system;Password=SUA_SENHA_AQUI;Data Source=localhost:1521/CHRONOS_DB"
  }
}

## 📂 Estrutura de Pastas

```
backend-dotnet/
├── Controllers/
│   └── NodesController.cs       # Endpoints REST: GET, POST, DELETE de nós DTN
├── Domain/
│   ├── Node.cs                  # Entidade: Nó da rede cislunar (gateway/relay)
│   ├── NodeLink.cs              # Entidade: Link de topologia entre nós
│   └── AssetBalance.cs          # Entidade: Saldo de ativos digitais por nó
├── Infrastructure/
│   └── ChronosDbContext.cs      # DbContext EF Core conectado ao Oracle
├── Program.cs                   # Ponto de entrada, DI e configuração do pipeline
├── appsettings.json             # String de conexão e configurações de ambiente
└── ChronosDotnetApi.csproj      # Arquivo de projeto .NET
```

---

## ▶️ Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado
- Oracle Database XE 21c em execução (use o módulo `devops/` com Docker Compose)

### 1. Configurar a string de conexão

Edite o arquivo `appsettings.json` e ajuste a `DefaultConnection` com os dados do seu Oracle:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=system;Password=ChronosSecurePassword2026;Data Source=localhost:1521/CHRONOS_DB"
  }
}
```

### 2. Restaurar dependências e executar

```bash
dotnet restore
dotnet run
```

### 3. Acessar a documentação Swagger

Após iniciar, acesse:

```
http://localhost:5000/swagger
```

---

## 🔗 Endpoints Principais

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/nodes` | Lista todos os nós ativos na rede DTN |
| `GET` | `/api/nodes/{id}` | Retorna detalhes de um nó específico |
| `POST` | `/api/nodes` | Registra um novo nó gateway na rede |
| `DELETE` | `/api/nodes/{id}` | Remove um nó (bloqueado se houver buffer pendente) |

---

## 🔗 Repositórios do Projeto Chronos DTN

| Módulo | Descrição |
|---|---|
| [backend-java](https://github.com/seu-usuario/chronos-backend-java) | API principal Spring Boot 3 + JWT |
| [database](https://github.com/seu-usuario/chronos-database) | Scripts Oracle SQL e Procedure PL/SQL |
| [devops](https://github.com/seu-usuario/chronos-devops) | Docker Compose e Dockerfile |
| [iot-esp32](https://github.com/seu-usuario/chronos-iot-esp32) | Firmware C++ Arduino para ESP32 |
| [mobile-app](https://github.com/seu-usuario/chronos-mobile-app) | App React Native com Expo Router |

---

## 👤 Autores

Projeto desenvolvido para a **Global Solution — FIAP 2026**.
