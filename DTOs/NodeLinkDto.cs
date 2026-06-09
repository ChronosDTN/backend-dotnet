namespace ChronosDotnetApi.DTOs;

public class NodeLinkDto {
    public int IdRoute { get; set; }
    public int SourceNodeId { get; set; }
    public int TargetNodeId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal BandwidthKbps { get; set; }
}
