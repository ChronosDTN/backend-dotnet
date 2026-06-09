namespace ChronosDotnetApi.Services;

using ChronosDotnetApi.Domain;
using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class AssetBalanceService : IAssetBalanceService {
    private readonly ChronosDbContext _context;
    private readonly ILogger<AssetBalanceService> _logger;

    public AssetBalanceService(ChronosDbContext context, ILogger<AssetBalanceService> logger) {
        _context = context;
        _logger = logger;
    }

    private static AssetBalanceDto ToDto(AssetBalance b) => new() {
        IdAsset = b.IdAsset,
        IdNode = b.IdNode,
        Symbol = b.Symbol,
        Balance = b.Balance,
        LastUpdate = b.LastUpdate
    };

    public async Task<PagedResultDto<AssetBalanceDto>> GetAllAsync(int page, int pageSize, int? nodeId) {
        var query = _context.AssetBalances.AsQueryable();
        if (nodeId.HasValue) {
            query = query.Where(b => b.IdNode == nodeId.Value);
        }
        var total = await query.CountAsync();
        var balances = await query
            .OrderBy(b => b.IdAsset)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<AssetBalanceDto> {
            Items = balances.Select(ToDto),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<AssetBalanceDto?> GetByIdAsync(int id) {
        var balance = await _context.AssetBalances.FindAsync(id);
        return balance == null ? null : ToDto(balance);
    }

    public async Task<AssetBalanceDto> CreateAsync(AssetBalanceCreateDto dto) {
        var nodeExists = await _context.Nodes.AnyAsync(n => n.IdNode == dto.IdNode);
        if (!nodeExists) {
            throw new KeyNotFoundException($"Nó com ID {dto.IdNode} não encontrado.");
        }

        var balance = new AssetBalance {
            IdNode = dto.IdNode,
            Symbol = dto.Symbol,
            Balance = dto.Balance,
            LastUpdate = DateTime.UtcNow
        };

        _context.AssetBalances.Add(balance);

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Erro ao criar saldo para o nó {NodeId}.", dto.IdNode);
            throw;
        }

        return ToDto(balance);
    }

    public async Task<bool> UpdateAsync(int id, AssetBalanceUpdateDto dto) {
        var balance = await _context.AssetBalances.FindAsync(id);
        if (balance == null) return false;

        balance.Symbol = dto.Symbol;
        balance.Balance = dto.Balance;
        balance.LastUpdate = DateTime.UtcNow;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Erro ao atualizar saldo com ID {AssetId}.", id);
            throw;
        }

        return true;
    }

    public async Task<bool> DeleteAsync(int id) {
        var balance = await _context.AssetBalances.FindAsync(id);
        if (balance == null) return false;

        _context.AssetBalances.Remove(balance);

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Erro ao deletar saldo com ID {AssetId}.", id);
            throw;
        }

        return true;
    }
}
