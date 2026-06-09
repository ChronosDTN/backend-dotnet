namespace ChronosDotnetApi.DTOs;

public class AssetBalanceDto {
    public int IdAsset { get; set; }
    public int IdNode { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime LastUpdate { get; set; }
}
