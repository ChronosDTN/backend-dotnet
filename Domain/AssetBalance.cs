namespace ChronosDotnetApi.Domain; // Namespace da camada de dominio.

using System.ComponentModel.DataAnnotations; // Bibliotecas de anotacoes de propriedades do .NET Core.
using System.ComponentModel.DataAnnotations.Schema; // Bibliotecas para mapeamento explicito de relacionamentos.

[Table("T_CDTN_ASSET_BALANCES")] // Define a tabela fisica correspondente aos saldos.
public class AssetBalance { // Declaracao da classe AssetBalance.

    [Key] // Chave primaria fisica autogerada.
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-incremento definido no banco (Identity).
    [Column("id_asset")] // ID do ativo.
    public int IdAsset { get; set; } // Identificador numerico do saldo.

    [Column("id_node")] // Chave estrangeira que referencia a tabela de gateways de rede.
    public int IdNode { get; set; } // Codigo do no associado.

    [Required] // Campo de simbolo de stablecoin obrigatorio.
    [MaxLength(10)] // Limite de caracteres para a sigla (USDC/USDT).
    [Column("symbol")] // Nome da coluna fisica.
    public string Symbol { get; set; } = string.Empty; // Simbolo comercial do token.

    [Required] // Saldo numerico financeiro obrigatorio.
    [Column("balance", TypeName = "decimal(18, 6)")] // Define precisao decimal para guardar 6 decimais do token.
    public decimal Balance { get; set; } = 0.000000M; // Valor liquido do saldo inicial.

    [Column("last_update")] // Carimbo de ultima movimentacao de saldo.
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow; // Atribui data atual padrao.

    // N:1 Relationship Configuration
    // Propriedade de navegacao apontando para o objeto pai de gateway correspondente.
    [ForeignKey("IdNode")] // Associa a chave estrangeira declarada no atributo IdNode.
    public virtual Node? Node { get; set; } // Objeto gateway.
} // Fim da classe AssetBalance.
