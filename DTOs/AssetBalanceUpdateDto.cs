namespace ChronosDotnetApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class AssetBalanceUpdateDto {
    [Required(ErrorMessage = "O símbolo do ativo é obrigatório.")]
    [StringLength(10, MinimumLength = 2, ErrorMessage = "O símbolo deve ter entre 2 e 10 caracteres.")]
    public string Symbol { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "O saldo não pode ser negativo.")]
    public decimal Balance { get; set; } = 0.000000M;
}
