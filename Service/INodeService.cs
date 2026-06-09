namespace ChronosDotnetApi.Services;

using ChronosDotnetApi.DTOs;

public interface INodeService {
    Task<PagedResultDto<NodeDto>> GetAllNodesAsync(int page, int pageSize);
    Task<NodeDto?> GetNodeByIdAsync(int id);
    Task<NodeDto> CreateNodeAsync(NodeCreateDto dto);
    Task<bool> UpdateNodeAsync(int id, NodeUpdateDto dto);
    Task<bool> DeleteNodeAsync(int id);
}