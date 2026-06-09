using ChronosDotnetApi.Domain;
using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Infrastructure;
using ChronosDotnetApi.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ChronosDotnetApi.Tests;

/// <summary>
/// Testes de integração do NodeService usando banco de dados em memória.
/// </summary>
public class NodeServiceTests : IDisposable {

    private readonly ChronosDbContext _context;
    private readonly NodeService _service;

    public NodeServiceTests() {
        var options = new DbContextOptionsBuilder<ChronosDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ChronosDbContext(options);
        _service = new NodeService(_context);
    }

    public void Dispose() {
        _context.Dispose();
    }

    // ─── CreateNodeAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateNodeAsync_DevePersistirNoNoContexto() {
        // Arrange
        var dto = new NodeCreateDto {
            Name = "Gateway Terra-1",
            Location = "Terra",
            NetworkAddress = "192.168.1.1"
        };

        // Act
        var result = await _service.CreateNodeAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Gateway Terra-1");
        result.Location.Should().Be("Terra");
        result.IdNode.Should().BeGreaterThan(0);

        var noNoBanco = await _context.Nodes.FindAsync(result.IdNode);
        noNoBanco.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateNodeAsync_DeveRetornarListaDeBalancesVazia() {
        // Arrange
        var dto = new NodeCreateDto { Name = "Lua-1", Location = "Lua", NetworkAddress = "10.0.0.1" };

        // Act
        var result = await _service.CreateNodeAsync(dto);

        // Assert
        result.Balances.Should().BeEmpty();
    }

    // ─── GetAllNodesAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllNodesAsync_DeveRetornarTodosOsNos() {
        // Arrange
        _context.Nodes.AddRange(
            new Node { Name = "Terra-1", Location = "Terra", NetworkAddress = "192.168.1.1" },
            new Node { Name = "Lua-1",   Location = "Lua",   NetworkAddress = "10.0.1.1" },
            new Node { Name = "Sat-1",   Location = "Orbita",NetworkAddress = "172.16.0.1" }
        );
        await _context.SaveChangesAsync();

        // Act
        var nodes = await _service.GetAllNodesAsync();

        // Assert
        nodes.Should().HaveCount(3);
    }

    // ─── GetNodeByIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetNodeByIdAsync_DeveRetornarNoCorreto() {
        // Arrange
        var node = new Node { Name = "Gateway Terra-1", Location = "Terra", NetworkAddress = "192.168.1.1" };
        _context.Nodes.Add(node);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetNodeByIdAsync(node.IdNode);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Gateway Terra-1");
    }

    [Fact]
    public async Task GetNodeByIdAsync_DeveRetornarNullQuandoNaoExiste() {
        // Act
        var result = await _service.GetNodeByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    // ─── Relacionamento 1:N (Node → AssetBalance) ─────────────────────────────

    [Fact]
    public async Task GetNodeByIdAsync_DeveRetornarBalancesAssociados() {
        // Arrange
        var node = new Node { Name = "Terra-1", Location = "Terra", NetworkAddress = "192.168.1.1" };
        _context.Nodes.Add(node);
        await _context.SaveChangesAsync();

        _context.AssetBalances.AddRange(
            new AssetBalance { IdNode = node.IdNode, Symbol = "USDC", Balance = 50000M },
            new AssetBalance { IdNode = node.IdNode, Symbol = "USDT", Balance = 25000M }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetNodeByIdAsync(node.IdNode);

        // Assert
        result!.Balances.Should().HaveCount(2);
        result.Balances.Should().Contain(b => b.Symbol == "USDC" && b.Balance == 50000M);
        result.Balances.Should().Contain(b => b.Symbol == "USDT" && b.Balance == 25000M);
    }

    // ─── UpdateNodeAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateNodeAsync_DeveAtualizarCamposCorretamente() {
        // Arrange
        var node = new Node { Name = "Original", Location = "Terra", NetworkAddress = "1.1.1.1" };
        _context.Nodes.Add(node);
        await _context.SaveChangesAsync();

        var dto = new NodeUpdateDto { Name = "Atualizado", Location = "Lua", NetworkAddress = "2.2.2.2" };

        // Act
        var success = await _service.UpdateNodeAsync(node.IdNode, dto);

        // Assert
        success.Should().BeTrue();
        var atualizado = await _context.Nodes.FindAsync(node.IdNode);
        atualizado!.Name.Should().Be("Atualizado");
        atualizado.Location.Should().Be("Lua");
        atualizado.NetworkAddress.Should().Be("2.2.2.2");
    }

    [Fact]
    public async Task UpdateNodeAsync_DeveRetornarFalseQuandoNaoExiste() {
        // Arrange
        var dto = new NodeUpdateDto { Name = "X", Location = "Y", NetworkAddress = "Z" };

        // Act
        var success = await _service.UpdateNodeAsync(999, dto);

        // Assert
        success.Should().BeFalse();
    }

    // ─── DeleteNodeAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteNodeAsync_DeveDeletarNoDoContexto() {
        // Arrange
        var node = new Node { Name = "Para Deletar", Location = "Terra", NetworkAddress = "9.9.9.9" };
        _context.Nodes.Add(node);
        await _context.SaveChangesAsync();

        // Act
        var success = await _service.DeleteNodeAsync(node.IdNode);

        // Assert
        success.Should().BeTrue();
        var deletado = await _context.Nodes.FindAsync(node.IdNode);
        deletado.Should().BeNull();
    }

    [Fact]
    public async Task DeleteNodeAsync_DeveRetornarFalseQuandoNaoExiste() {
        // Act
        var success = await _service.DeleteNodeAsync(999);

        // Assert
        success.Should().BeFalse();
    }

    // ─── Cascade Delete (1:N) ─────────────────────────────────────────────────

    [Fact]
    public async Task DeleteNodeAsync_DeveDeletarBalancesEmCascata() {
        // Arrange
        var node = new Node { Name = "Terra-Cascade", Location = "Terra", NetworkAddress = "5.5.5.5" };
        _context.Nodes.Add(node);
        await _context.SaveChangesAsync();

        _context.AssetBalances.Add(new AssetBalance { IdNode = node.IdNode, Symbol = "USDC", Balance = 1000M });
        await _context.SaveChangesAsync();

        var nodeId = node.IdNode;

        // Act
        await _service.DeleteNodeAsync(nodeId);

        // Assert
        var balancesRestantes = _context.AssetBalances.Where(b => b.IdNode == nodeId);
        balancesRestantes.Should().BeEmpty();
    }
}
