namespace ChronosDotnetApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class NodeLinkCreateDto {
    [Required(ErrorMessage = "O ID do nó de origem é obrigatório.")]
    public int SourceNodeId { get; set; }

    [Required(ErrorMessage = "O ID do nó de destino é obrigatório.")]
    public int TargetNodeId { get; set; }

    [Required(ErrorMessage = "O status do enlace é obrigatório.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "O status deve ter entre 2 e 20 caracteres.")]
    public string Status { get; set; } = "ACTIVE";

    [Range(0.01, double.MaxValue, ErrorMessage = "A largura de banda deve ser maior que zero.")]
    public decimal BandwidthKbps { get; set; } = 1024.00M;
}
