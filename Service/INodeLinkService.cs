namespace ChronosDotnetApi.Services;

using ChronosDotnetApi.DTOs;

public interface INodeLinkService {
    Task<PagedResultDto<NodeLinkDto>> GetAllAsync(int page, int pageSize);
    Task<NodeLinkDto?> GetByIdAsync(int id);
    Task<NodeLinkDto> CreateAsync(NodeLinkCreateDto dto);
    Task<bool> UpdateAsync(int id, NodeLinkUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
