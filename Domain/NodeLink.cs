namespace ChronosDotnetApi.Domain; // Namespace da pasta de modelos de Dominio.

using System.ComponentModel.DataAnnotations; // Usings para mapeamento e validacoes EF Core.
using System.ComponentModel.DataAnnotations.Schema; // Usings de atributos fisicos de tabela relacional.

[Table("T_CDTN_DYNAMIC_ROUTES")] // Mapeia para a tabela de rotas dinâmicas de sinal espacial.
public class NodeLink { // Classe de associacao N:N para rotas.

    [Key] // Chave primaria de identificacao de rota.
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-incremento.
    [Column("id_route")] // ID da rota fisica.
    public int IdRoute { get; set; } // Identificador da ligacao.

    [Column("source_node")] // Chave estrangeira referenciando o no emissor do enlace.
    public int SourceNodeId { get; set; } // ID de origem.

    [Column("target_node")] // Chave estrangeira referenciando o no receptor do enlace.
    public int TargetNodeId { get; set; } // ID de destino.

    [Required] // Campo descritivo de conexao obrigatorio.
    [MaxLength(20)] // Limite de tamanho da string.
    [Column("status")] // Nome da coluna fisica.
    public string Status { get; set; } = "ACTIVE"; // Status do enlace (ex: ACTIVE, OFFLINE_SHADOW).

    [Column("bandwidth_kbps")] // Largura de banda do laser espacial.
    public decimal BandwidthKbps { get; set; } = 1024.00M; // Velocidade de transferencia.

    // N:N Relationship Navigation Configuration
    // Propriedade de navegacao para o no de origem do sinal físico.
    [ForeignKey("SourceNodeId")] // Associa a chave de origem.
    public virtual Node? SourceNode { get; set; } // Objeto no de origem.

    // Propriedade de navegacao para o no de destino do sinal físico.
    [ForeignKey("TargetNodeId")] // Associa a chave de destino.
    public virtual Node? TargetNode { get; set; } // Objeto no de destino.
} // Fim da classe NodeLink.
