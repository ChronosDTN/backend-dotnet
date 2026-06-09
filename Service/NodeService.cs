namespace ChronosDotnetApi.Services;

using ChronosDotnetApi.Domain;
using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class NodeService : INodeService {
    private readonly ChronosDbContext _context;
    private readonly ILogger<NodeService> _logger;

    public NodeService(ChronosDbContext context, ILogger<NodeService> logger) {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResultDto<NodeDto>> GetAllNodesAsync(int page, int pageSize) {
        var query = _context.Nodes.Include(n => n.Balances);
        var total = await query.CountAsync();
        var nodes = await query
            .OrderBy(n => n.IdNode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<NodeDto> {
            Items = nodes.Select(n => new NodeDto {
                IdNode = n.IdNode,
                Name = n.Name,
                Location = n.Location,
                NetworkAddress = n.NetworkAddress,
                CreatedAt = n.CreatedAt,
                Balances = n.Balances.Select(b => new AssetBalanceDto {
                    IdAsset = b.IdAsset,
                    IdNode = b.IdNode,
                    Symbol = b.Symbol,
                    Balance = b.Balance,
                    LastUpdate = b.LastUpdate
                }).ToList()
            }),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<NodeDto?> GetNodeByIdAsync(int id) {
        var node = await _context.Nodes
            .Include(n => n.Balances)
            .FirstOrDefaultAsync(n => n.IdNode == id);

        if (node == null) return null;

        return new NodeDto {
            IdNode = node.IdNode,
            Name = node.Name,
            Location = node.Location,
            NetworkAddress = node.NetworkAddress,
            CreatedAt = node.CreatedAt,
            Balances = node.Balances.Select(b => new AssetBalanceDto {
                IdAsset = b.IdAsset,
                IdNode = b.IdNode,
                Symbol = b.Symbol,
                Balance = b.Balance,
                LastUpdate = b.LastUpdate
            }).ToList()
        };
    }

    public async Task<NodeDto> CreateNodeAsync(NodeCreateDto dto) {
        var node = new Node {
            Name = dto.Name,
            Location = dto.Location,
            NetworkAddress = dto.NetworkAddress,
            CreatedAt = DateTime.UtcNow
        };

        _context.Nodes.Add(node);

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Erro ao criar nó com NetworkAddress '{NetworkAddress}'.", dto.NetworkAddress);
            throw;
        }

        return new NodeDto {
            IdNode = node.IdNode,
            Name = node.Name,
            Location = node.Location,
            NetworkAddress = node.NetworkAddress,
            CreatedAt = node.CreatedAt,
            Balances = new List<AssetBalanceDto>()
        };
    }

    public async Task<bool> UpdateNodeAsync(int id, NodeUpdateDto dto) {
        var node = await _context.Nodes.FindAsync(id);
        if (node == null) return false;

        node.Name = dto.Name;
        node.Location = dto.Location;
        node.NetworkAddress = dto.NetworkAddress;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Erro ao atualizar nó com ID {NodeId}.", id);
            throw;
        }

        return true;
    }

    public async Task<bool> DeleteNodeAsync(int id) {
        var node = await _context.Nodes.FindAsync(id);
        if (node == null) return false;

        _context.Nodes.Remove(node);

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Erro ao deletar nó com ID {NodeId}.", id);
            throw;
        }

        return true;
    }
}