namespace ChronosDotnetApi.Services;

using ChronosDotnetApi.Domain;
using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class NodeService : INodeService {
    private readonly ChronosDbContext _context;

    public NodeService(ChronosDbContext context) {
        _context = context;
    }

    public async Task<IEnumerable<NodeDto>> GetAllNodesAsync() {
        var nodes = await _context.Nodes
            .Include(n => n.Balances)
            .ToListAsync();

        return nodes.Select(n => new NodeDto {
            IdNode = n.IdNode,
            Name = n.Name,
            Location = n.Location,
            NetworkAddress = n.NetworkAddress,
            CreatedAt = n.CreatedAt,
            Balances = n.Balances.Select(b => new AssetBalanceDto {
                IdAsset = b.IdAsset,
                Symbol = b.Symbol,
                Balance = b.Balance,
                LastUpdate = b.LastUpdate
            }).ToList()
        });
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
        await _context.SaveChangesAsync();

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

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteNodeAsync(int id) {
        var node = await _context.Nodes.FindAsync(id);
        if (node == null) return false;

        _context.Nodes.Remove(node);
        await _context.SaveChangesAsync();
        return true;
    }
}