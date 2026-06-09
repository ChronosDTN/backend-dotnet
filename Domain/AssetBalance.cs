namespace ChronosDotnetApi.Domain;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("T_CDTN_ASSET_BALANCES")]
public class AssetBalance {

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_asset")]
    public int IdAsset { get; set; }

    [Column("id_node")]
    public int IdNode { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [Column("balance", TypeName = "decimal(18, 6)")]
    public decimal Balance { get; set; } = 0.000000M;

    [Column("last_update")]
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    [ForeignKey("IdNode")]
    public virtual Node? Node { get; set; }
}

