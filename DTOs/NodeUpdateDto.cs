namespace ChronosDotnetApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class NodeUpdateDto {
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NetworkAddress { get; set; } = string.Empty;
}
