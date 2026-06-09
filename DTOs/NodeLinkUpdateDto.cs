namespace ChronosDotnetApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class NodeLinkUpdateDto {
    [Required(ErrorMessage = "O status do enlace é obrigatório.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "O status deve ter entre 2 e 20 caracteres.")]
    public string Status { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "A largura de banda deve ser maior que zero.")]
    public decimal BandwidthKbps { get; set; }
}
