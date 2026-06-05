namespace ChronosDotnetApi.Infrastructure; // Namespace da camada de infraestrutura e dados do Clean Architecture.

using ChronosDotnetApi.Domain; // Importa as entidades de dominio para mapeamento no contexto.
using Microsoft.EntityFrameworkCore; // Importa os componentes do Entity Framework Core.

public class ChronosDbContext : DbContext { // Declaracao da classe principal de contexto.

    public ChronosDbContext(DbContextOptions<ChronosDbContext> options) : base(options) {
        // Construtor padrao recebendo configuracoes de conexao do container ASP.NET Core.
    } // Fim do construtor.

    // Colecao de mapeamento e acesso a tabela de Gateways de Rede (Nodes).
    public DbSet<Node> Nodes { get; set; } = null!;
    // Colecao de mapeamento e acesso a tabela de Saldos de Ativos (AssetBalances).
    public DbSet<AssetBalance> AssetBalances { get; set; } = null!;
    // Colecao de mapeamento e acesso a tabela de Rotas/Links Dinâmicos (NodeLinks).
    public DbSet<NodeLink> NodeLinks { get; set; } = null!;

    @Override // Sobrescrita do metodo de definicao e construcao de modelos do EF Core.
    protected void OnModelCreating(ModelBuilder modelBuilder) { // Metodo OnModelCreating.
        
        // Configura chave primaria composta ou chaves alternativas se necessario (baseado em anotações).
        base.OnModelCreating(modelBuilder); // Chama execucao padrão do EF Core.

        // 1:N Relationship Configuration (Node -> AssetBalances)
        modelBuilder.Entity<AssetBalance>() // Habilita configuracoes para a entidade de saldos.
            .HasOne(ab => ab.Node) // Informa que cada saldo pertence a exatamente 1 No.
            .WithMany(n => n.Balances) // Informa que cada No possui N saldos cadastrados.
            .HasForeignKey(ab => ab.IdNode) // Configura a chave estrangeira fisica no banco.
            .OnDelete(DeleteBehavior.Cascade); // Determina delecao em cascata ao excluir o no pai.

        // N:N Relationship Routing Configuration (Node -> Node via NodeLink)
        modelBuilder.Entity<NodeLink>() // Configura a entidade de juncao N:N.
            .HasOne(nl => nl.SourceNode) // Configura o relacionamento de origem de sinal.
            .WithMany(n => n.OutgoingLinks) // Informa que o No de origem possui N links de saida.
            .HasForeignKey(nl => nl.SourceNodeId) // Especifica chave estrangeira de origem.
            .OnDelete(DeleteBehavior.Restrict); // Evita exclusao recursiva multipla do mesmo no.

        modelBuilder.Entity<NodeLink>() // Configura a outra ponta do relacionamento de juncao.
            .HasOne(nl => nl.TargetNode) // Configura o relacionamento de destino do sinal.
            .WithMany(n => n.IncomingLinks) // Informa que o No de destino possui N links de entrada.
            .HasForeignKey(nl => nl.TargetNodeId) // Especifica chave estrangeira de destino.
            .OnDelete(DeleteBehavior.Restrict); // Evita exclusao recursiva multipla.
    } // Fim do metodo OnModelCreating.
} // Fim do DbContext.
