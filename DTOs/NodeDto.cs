namespace ChronosDotnetApi.DTOs;

public class NodeDto {
    public int IdNode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string NetworkAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<AssetBalanceDto> Balances { get; set; } = new();
}
