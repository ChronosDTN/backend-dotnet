namespace ChronosDotnetApi.Domain; // Define o namespace para a camada de Dominio no Clean Architecture.

using System.ComponentModel.DataAnnotations; // Importa validacoes basicas de atributos de modelo.
using System.ComponentModel.DataAnnotations.Schema; // Importa annotations de mapeamento de banco fisico.
using System.Text.Json.Serialization; // Importa serializador para ignorar referencias circulares em APIs.

[Table("T_CDTN_NODE_REGISTRY")] // Mapeia a classe C# para a tabela fisica no banco Oracle.
public class Node { // Declaracao da entidade Node.

    [Key] // Anota o atributo como chave primaria da tabela.
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Define geracao automatica pelo banco (Identity).
    [Column("id_node")] // Nome fisico correspondente da coluna no banco de dados.
    public int IdNode { get; set; } // Identificador numerico do no.

    [Required] // Valida que o nome do no de rede e de preenchimento obrigatorio.
    [MaxLength(100)] // Restringe o tamanho maximo a 100 caracteres.
    [Column("name")] // Coluna fisica de persistencia.
    public string Name { get; set; } = string.Empty; // Nome amigavel do gateway.

    [Required] // Valida que a localizacao geografica nao pode vir nula.
    [MaxLength(50)] // Restringe o tamanho maximo do campo.
    [Column("location")] // Coluna fisica contendo regiao (Terra/Lua).
    public string Location { get; set; } = string.Empty; // Descricao de localizacao espacial.

    [Required] // Campo obrigatorio para endereçamento IP ou dns.
    [MaxLength(100)] // Tamanho limite da string.
    [Column("network_address")] // Nome fisico da coluna.
    public string NetworkAddress { get; set; } = string.Empty; // Endereco de rede unico do gateway.

    [Column("created_at")] // Carimbo de data de criacao do registro.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Atribui valor inicial padrao em UTC.

    // 1:N Relationship Configuration (Mapeamento de 1 para N)
    // Colecao contendo os saldos de stablecoins que pertencem ou sao custodiados por este no de gateway.
    [JsonIgnore] // Impede que o serializador Jackson/Newtonsoft entre em loop infinito de relacionamentos.
    public virtual ICollection<AssetBalance> Balances { get; set; } = new List<AssetBalance>(); // Saldo de stablecoin.

    // N:N Relationship Configuration (Relacionamento de N para N - Roteamento entre gateways)
    // Colecao contendo as rotas de ligacoes em que este no atua como origem de sinal.
    [JsonIgnore] // Evita loops de serializacao JSON na api REST.
    public virtual ICollection<NodeLink> OutgoingLinks { get; set; } = new List<NodeLink>(); // Links de saida.

    // Colecao contendo as rotas de ligacoes em que este no atua como destino final do sinal de rede.
    [JsonIgnore] // Evita loops de serializacao JSON na api REST.
    public virtual ICollection<NodeLink> IncomingLinks { get; set; } = new List<NodeLink>(); // Links de entrada.
} // Fim da definicao da classe Node.
