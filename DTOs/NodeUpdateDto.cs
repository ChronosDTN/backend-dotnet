namespace ChronosDotnetApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class NodeUpdateDto {
    [Required(ErrorMessage = "O nome do nó é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "A localização é obrigatória.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "A localização deve ter entre 2 e 50 caracteres.")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "O endereço de rede é obrigatório.")]
    [StringLength(100, MinimumLength = 7, ErrorMessage = "O endereço de rede deve ter entre 7 e 100 caracteres.")]
    [RegularExpression(
        @"^(\d{1,3}\.){3}\d{1,3}$|^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)*[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?$",
        ErrorMessage = "O endereço de rede deve ser um IPv4 válido (ex: 192.168.1.1) ou um hostname DNS.")]
    public string NetworkAddress { get; set; } = string.Empty;
}
