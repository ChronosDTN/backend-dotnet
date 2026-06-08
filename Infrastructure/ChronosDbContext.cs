namespace ChronosDotnetApi.Infrastructure; // Namespace da camada de infraestrutura e dados.

using ChronosDotnetApi.Domain; // Importa as entidades de dominio para mapeamento no contexto.
using Microsoft.EntityFrameworkCore; // Importa os componentes do Entity Framework Core.

/// <summary>
/// DbContext principal do Chronos DTN — gerencia o acesso ao Oracle XE e
/// configura os relacionamentos entre entidades via Fluent API.
/// </summary>
public class ChronosDbContext : DbContext {

    /// <summary>
    /// Construtor padrão recebendo as opções de conexão injetadas pelo container ASP.NET Core.
    /// </summary>
    public ChronosDbContext(DbContextOptions<ChronosDbContext> options) : base(options) { }

    /// <summary>Coleção mapeada para a tabela T_CDTN_NODE_REGISTRY (gateways de rede).</summary>
    public DbSet<Node> Nodes { get; set; } = null!;

    /// <summary>Coleção mapeada para a tabela T_CDTN_ASSET_BALANCES (saldos de stablecoins).</summary>
    public DbSet<AssetBalance> AssetBalances { get; set; } = null!;

    /// <summary>Coleção mapeada para a tabela T_CDTN_DYNAMIC_ROUTES (links de topologia N:N).</summary>
    public DbSet<NodeLink> NodeLinks { get; set; } = null!;

    /// <summary>
    /// Configura os relacionamentos e comportamentos de deleção via Fluent API do EF Core.
    /// A keyword <c>override</c> em C# é OBRIGATÓRIA na assinatura para que o EF Core
    /// invoque este método durante a construção do modelo — diferente do Java onde
    /// <c>@Override</c> é apenas uma anotação opcional de verificação do compilador.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {

        // Chama a implementação base do EF Core antes de aplicar configurações customizadas.
        base.OnModelCreating(modelBuilder);

        // ─── Relacionamento 1:N — Node → AssetBalance ──────────────────────────────
        // Cascade: ao deletar um Node, todos os AssetBalances filhos são removidos automaticamente.
        // Justificativa: saldo de stablecoin não tem sentido de existir sem o nó gateway pai.
        modelBuilder.Entity<AssetBalance>()
            .HasOne(ab => ab.Node)
            .WithMany(n => n.Balances)
            .HasForeignKey(ab => ab.IdNode)
            .OnDelete(DeleteBehavior.Cascade);

        // ─── Relacionamento N:N — Node ↔ Node via NodeLink ─────────────────────────
        // Restrict na origem: impede deletar um Node que ainda tem rotas de saída ativas.
        // Restrict no destino: impede deletar um Node que ainda é destino de rotas ativas.
        // Justificativa: DeleteBehavior.Cascade em ambas as FKs da mesma tabela causaria
        // "multiple cascade paths" — erro do SQL Server/Oracle. Restrict evita deleção
        // acidental de nós com conectividade ativa na topologia cislunar.
        modelBuilder.Entity<NodeLink>()
            .HasOne(nl => nl.SourceNode)
            .WithMany(n => n.OutgoingLinks)
            .HasForeignKey(nl => nl.SourceNodeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NodeLink>()
            .HasOne(nl => nl.TargetNode)
            .WithMany(n => n.IncomingLinks)
            .HasForeignKey(nl => nl.TargetNodeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
