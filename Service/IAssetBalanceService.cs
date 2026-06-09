namespace ChronosDotnetApi.Services;

using ChronosDotnetApi.DTOs;

public interface IAssetBalanceService {
    Task<PagedResultDto<AssetBalanceDto>> GetAllAsync(int page, int pageSize, int? nodeId);
    Task<AssetBalanceDto?> GetByIdAsync(int id);
    Task<AssetBalanceDto> CreateAsync(AssetBalanceCreateDto dto);
    Task<bool> UpdateAsync(int id, AssetBalanceUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
