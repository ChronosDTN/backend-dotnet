# 📦 Documentação de Migrations — Chronos DTN

Este documento explica o que são Migrations do Entity Framework Core, como foram utilizadas neste projeto e como gerenciá-las.

---

## 📖 O que são Migrations?

**Migrations** são uma forma de versionar o esquema do banco de dados de forma incremental e rastreável, diretamente a partir do código C#.

Em vez de escrever SQL manualmente para criar ou alterar tabelas, o EF Core:
1. Lê as classes de domínio (`Node`, `AssetBalance`, `NodeLink`)
2. Compara com o estado atual do banco de dados
3. Gera automaticamente o SQL necessário para atualizar o banco

### Por que são importantes neste projeto?

- **Controle de versão do banco:** cada migration é um arquivo rastreado pelo Git
- **Reproducibilidade:** qualquer desenvolvedor pode recriar o banco exato com `dotnet ef database update`
- **Sem SQL manual:** as tabelas Oracle (`T_CDTN_NODE_REGISTRY`, etc.) foram criadas inteiramente via migrations
- **Rollback seguro:** é possível reverter o banco a um estado anterior se uma migration quebrar algo

---

## 🛠️ Comandos Úteis

> Todos os comandos devem ser executados na raiz do projeto.

### Criar uma nova migration

```bash
dotnet ef migrations add NomeDaMigration
```

Exemplo:
```bash
dotnet ef migrations add AdicionaCampoStatusNoNode
```

Isso gera dois arquivos em `Migrations/`:
- `YYYYMMDDHHMMSS_AdicionaCampoStatusNoNode.cs` — as operações Up/Down
- `YYYYMMDDHHMMSS_AdicionaCampoStatusNoNode.Designer.cs` — snapshot interno do EF

---

### Aplicar migrations ao banco

```bash
dotnet ef database update
```

Aplica todas as migrations pendentes. Para aplicar até uma migration específica:

```bash
dotnet ef database update NomeDaMigration
```

---

### Reverter uma migration

Para reverter ao estado anterior à última migration:

```bash
dotnet ef database update NomeDaMigrationAnterior
```

Para reverter **todas** as migrations (banco vazio):

```bash
dotnet ef database update 0
```

> ⚠️ Isso executa o método `Down()` de cada migration em ordem inversa.

---

### Remover a última migration (antes de aplicar)

Se criou uma migration errada e ainda não aplicou ao banco:

```bash
dotnet ef migrations remove
```

---

### Ver migrations pendentes

```bash
dotnet ef migrations list
```

Exemplo de saída:
```
20260609124848_InitialCreate (Applied)
```

---

### Gerar script SQL (para revisar antes de aplicar)

```bash
dotnet ef migrations script
```

Ou de uma migration específica para outra:

```bash
dotnet ef migrations script InitialCreate AdicionaCampoStatus
```

---

## 📁 Estrutura das Migrations no Projeto

```
Migrations/
├── 20260609124848_InitialCreate.cs           ← operações Up/Down
├── 20260609124848_InitialCreate.Designer.cs  ← snapshot interno
└── ChronosDbContextModelSnapshot.cs          ← estado atual do modelo
```

### `20260609124848_InitialCreate.cs`

Contém dois métodos:

- **`Up(MigrationBuilder)`**: cria as tabelas `T_CDTN_NODE_REGISTRY`, `T_CDTN_ASSET_BALANCES` e `T_CDTN_DYNAMIC_ROUTES` com todas as colunas, PKs e FKs
- **`Down(MigrationBuilder)`**: desfaz tudo — dropa as tabelas na ordem correta respeitando as FKs

### `ChronosDbContextModelSnapshot.cs`

É um snapshot do estado atual do modelo C#. O EF Core usa este arquivo para calcular o **diff** quando você cria uma nova migration — ele compara o snapshot com as classes atuais de domínio e gera apenas as alterações necessárias.

> ⚠️ **Nunca edite este arquivo manualmente.** Ele é gerado e atualizado automaticamente pelo `dotnet ef`.

---

## ✅ Boas Práticas

### Quando criar uma nova migration?

- Sempre que adicionar, remover ou renomear propriedades em classes de domínio
- Sempre que alterar tipos de dados ou tamanhos máximos
- Sempre que adicionar novos relacionamentos
- **Nunca** acumule muitas mudanças em uma única migration — prefira migrations pequenas e atômicas

### Como nomear migrations?

Use nomes descritivos em inglês no formato `VerbSubstantivo`:

| ✅ Bom | ❌ Ruim |
|---|---|
| `AddStatusToNodeLink` | `migration1` |
| `CreateAssetBalanceTable` | `update` |
| `RenameNetworkAddressColumn` | `fix` |

### Como testar migrations antes de produção?

1. Gere o script SQL: `dotnet ef migrations script`
2. Revise o SQL gerado antes de executar
3. Aplique primeiro em ambiente de desenvolvimento
4. Use `dotnet ef database update --connection "...string de homologação..."` para homologação
5. Só então aplique em produção

### Migrations e ambientes múltiplos

Para aplicar em um banco de dados específico sem alterar o `appsettings.json`:

```bash
dotnet ef database update --connection "User Id=prod_user;Password=...;Data Source=oracle-prod:1521/XE;"
```
