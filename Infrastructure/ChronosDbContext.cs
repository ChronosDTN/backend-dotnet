namespace ChronosDotnetApi.Infrastructure;

using ChronosDotnetApi.Domain;
using Microsoft.EntityFrameworkCore;

public class ChronosDbContext : DbContext {

    public ChronosDbContext(DbContextOptions<ChronosDbContext> options) : base(options) {
    }

    // Colecao de mapeamento e acesso a tabela de Gateways de Rede 
    public DbSet<Node> Nodes { get; set; } = null!;
    // Colecao de mapeamento e acesso a tabela de Saldos de Ativos 
    public DbSet<AssetBalance> AssetBalances { get; set; } = null!;
    // Colecao de mapeamento e acesso a tabela de Rotas/Links Dinâmicos 
    public DbSet<NodeLink> NodeLinks { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        
        // Configura chave primaria composta ou chaves alternativas se necessario 
        base.OnModelCreating(modelBuilder);

        // 1:N Relationship Configuration (Node -> AssetBalances)
        modelBuilder.Entity<AssetBalance>()
            .HasOne(ab => ab.Node)
            .WithMany(n => n.Balances)
            .HasForeignKey(ab => ab.IdNode)
            .OnDelete(DeleteBehavior.Cascade); // Saldos sem nó não fazem sentido 

        // N:N Relationship Routing Configuration
        modelBuilder.Entity<NodeLink>()
            .HasOne(nl => nl.SourceNode)
            .WithMany(n => n.OutgoingLinks)
            .HasForeignKey(nl => nl.SourceNodeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NodeLink>()
            .HasOne(nl => nl.TargetNode)
            .WithMany(n => n.IncomingLinks)
            .HasForeignKey(nl => nl.TargetNodeId)
            .OnDelete(DeleteBehavior.Restrict); // Evita deleção acidental de topologia da rede
    }
}