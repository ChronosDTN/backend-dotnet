namespace ChronosDotnetApi.Services;

using ChronosDotnetApi.Domain;
using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class NodeLinkService : INodeLinkService {
    private readonly ChronosDbContext _context;
    private readonly ILogger<NodeLinkService> _logger;

    public NodeLinkService(ChronosDbContext context, ILogger<NodeLinkService> logger) {
        _context = context;
        _logger = logger;
    }

    private static NodeLinkDto ToDto(NodeLink nl) => new() {
        IdRoute = nl.IdRoute,
        SourceNodeId = nl.SourceNodeId,
        TargetNodeId = nl.TargetNodeId,
        Status = nl.Status,
        BandwidthKbps = nl.BandwidthKbps
    };

    public async Task<PagedResultDto<NodeLinkDto>> GetAllAsync(int page, int pageSize) {
        var query = _context.NodeLinks.AsQueryable();
        var total = await query.CountAsync();
        var links = await query
            .OrderBy(nl => nl.IdRoute)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<NodeLinkDto> {
            Items = links.Select(ToDto),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<NodeLinkDto?> GetByIdAsync(int id) {
        var link = await _context.NodeLinks.FindAsync(id);
        return link == null ? null : ToDto(link);
    }

    public async Task<NodeLinkDto> CreateAsync(NodeLinkCreateDto dto) {
        var sourceExists = await _context.Nodes.AnyAsync(n => n.IdNode == dto.SourceNodeId);
        if (!sourceExists) {
            throw new KeyNotFoundException($"Nó de origem com ID {dto.SourceNodeId} não encontrado.");
        }

        var targetExists = await _context.Nodes.AnyAsync(n => n.IdNode == dto.TargetNodeId);
        if (!targetExists) {
            throw new KeyNotFoundException($"Nó de destino com ID {dto.TargetNodeId} não encontrado.");
        }

        var link = new NodeLink {
            SourceNodeId = dto.SourceNodeId,
            TargetNodeId = dto.TargetNodeId,
            Status = dto.Status,
            BandwidthKbps = dto.BandwidthKbps
        };

        _context.NodeLinks.Add(link);

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Erro ao criar rota entre nós {Source} e {Target}.", dto.SourceNodeId, dto.TargetNodeId);
            throw;
        }

        return ToDto(link);
    }

    public async Task<bool> UpdateAsync(int id, NodeLinkUpdateDto dto) {
        var link = await _context.NodeLinks.FindAsync(id);
        if (link == null) return false;

        link.Status = dto.Status;
        link.BandwidthKbps = dto.BandwidthKbps;

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Erro ao atualizar rota com ID {RouteId}.", id);
            throw;
        }

        return true;
    }

    public async Task<bool> DeleteAsync(int id) {
        var link = await _context.NodeLinks.FindAsync(id);
        if (link == null) return false;

        _context.NodeLinks.Remove(link);

        try {
            await _context.SaveChangesAsync();
        } catch (DbUpdateException ex) {
            _logger.LogError(ex, "Erro ao deletar rota com ID {RouteId}.", id);
            throw;
        }

        return true;
    }
}
