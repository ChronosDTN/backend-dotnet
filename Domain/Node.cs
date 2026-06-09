namespace ChronosDotnetApi.Domain;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("T_CDTN_NODE_REGISTRY")]
public class Node {

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_node")]
    public int IdNode { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("location")]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("network_address")]
    public string NetworkAddress { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public virtual ICollection<AssetBalance> Balances { get; set; } = new List<AssetBalance>();

    [JsonIgnore]
    public virtual ICollection<NodeLink> OutgoingLinks { get; set; } = new List<NodeLink>();

    [JsonIgnore]
    public virtual ICollection<NodeLink> IncomingLinks { get; set; } = new List<NodeLink>();
}

