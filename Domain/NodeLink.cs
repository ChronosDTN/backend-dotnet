namespace ChronosDotnetApi.Domain;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("T_CDTN_DYNAMIC_ROUTES")]
public class NodeLink {

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_route")]
    public int IdRoute { get; set; }

    [Column("source_node")]
    public int SourceNodeId { get; set; }

    [Column("target_node")]
    public int TargetNodeId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "ACTIVE";

    [Column("bandwidth_kbps")]
    public decimal BandwidthKbps { get; set; } = 1024.00M;

    [ForeignKey("SourceNodeId")]
    public virtual Node? SourceNode { get; set; }

    [ForeignKey("TargetNodeId")]
    public virtual Node? TargetNode { get; set; }
}

