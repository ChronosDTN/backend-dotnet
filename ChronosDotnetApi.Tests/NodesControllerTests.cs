using ChronosDotnetApi.Controllers;
using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ChronosDotnetApi.Tests;

public class NodesControllerTests {

    private readonly Mock<INodeService> _mockService;
    private readonly NodesController _controller;

    public NodesControllerTests() {
        _mockService = new Mock<INodeService>();
        _controller = new NodesController(_mockService.Object);
    }

    // ─── GET /api/nodes ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetNodes_DeveRetornarOkComListaDeNos() {
        // Arrange
        var nodes = new List<NodeDto> {
            new() { IdNode = 1, Name = "Gateway Terra-1", Location = "Terra", NetworkAddress = "192.168.1.1" },
            new() { IdNode = 2, Name = "Gateway Lua-1",   Location = "Lua",   NetworkAddress = "10.0.1.1" }
        };
        _mockService.Setup(s => s.GetAllNodesAsync()).ReturnsAsync(nodes);

        // Act
        var result = await _controller.GetNodes();

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeAssignableTo<IEnumerable<NodeDto>>().Subject;
        body.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetNodes_DeveRetornarListaVaziaQuandoNaoHaNodes() {
        // Arrange
        _mockService.Setup(s => s.GetAllNodesAsync()).ReturnsAsync(new List<NodeDto>());

        // Act
        var result = await _controller.GetNodes();

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeAssignableTo<IEnumerable<NodeDto>>().Subject;
        body.Should().BeEmpty();
    }

    // ─── GET /api/nodes/{id} ──────────────────────────────────────────────────

    [Fact]
    public async Task GetNode_DeveRetornarOkQuandoNoExiste() {
        // Arrange
        var node = new NodeDto { IdNode = 1, Name = "Gateway Terra-1", Location = "Terra", NetworkAddress = "192.168.1.1" };
        _mockService.Setup(s => s.GetNodeByIdAsync(1)).ReturnsAsync(node);

        // Act
        var result = await _controller.GetNode(1);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<NodeDto>().Subject;
        body.IdNode.Should().Be(1);
        body.Name.Should().Be("Gateway Terra-1");
    }

    [Fact]
    public async Task GetNode_DeveRetornarNotFoundQuandoNoNaoExiste() {
        // Arrange
        _mockService.Setup(s => s.GetNodeByIdAsync(99)).ReturnsAsync((NodeDto?)null);

        // Act
        var result = await _controller.GetNode(99);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ─── POST /api/nodes ──────────────────────────────────────────────────────

    [Fact]
    public async Task PostNode_DeveRetornarCreatedComLocationHeader() {
        // Arrange
        var dto = new NodeCreateDto { Name = "Gateway Terra-1", Location = "Terra", NetworkAddress = "192.168.1.1" };
        var created = new NodeDto { IdNode = 1, Name = dto.Name, Location = dto.Location, NetworkAddress = dto.NetworkAddress };
        _mockService.Setup(s => s.CreateNodeAsync(dto)).ReturnsAsync(created);

        // Act
        var result = await _controller.PostNode(dto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(NodesController.GetNode));
        createdResult.RouteValues!["id"].Should().Be(1);
        var body = createdResult.Value.Should().BeOfType<NodeDto>().Subject;
        body.Name.Should().Be("Gateway Terra-1");
    }

    [Fact]
    public async Task PostNode_DeveRetornarBadRequestQuandoModelStateInvalido() {
        // Arrange
        var dto = new NodeCreateDto { Name = "", Location = "Terra", NetworkAddress = "192.168.1.1" };
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = await _controller.PostNode(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── PUT /api/nodes/{id} ──────────────────────────────────────────────────

    [Fact]
    public async Task PutNode_DeveRetornarNoContentQuandoAtualizaComSucesso() {
        // Arrange
        var dto = new NodeUpdateDto { Name = "Atualizado", Location = "Terra", NetworkAddress = "192.168.1.99" };
        _mockService.Setup(s => s.UpdateNodeAsync(1, dto)).ReturnsAsync(true);

        // Act
        var result = await _controller.PutNode(1, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task PutNode_DeveRetornarNotFoundQuandoNoNaoExiste() {
        // Arrange
        var dto = new NodeUpdateDto { Name = "X", Location = "Y", NetworkAddress = "Z" };
        _mockService.Setup(s => s.UpdateNodeAsync(99, dto)).ReturnsAsync(false);

        // Act
        var result = await _controller.PutNode(99, dto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task PutNode_DeveRetornarBadRequestQuandoModelStateInvalido() {
        // Arrange
        var dto = new NodeUpdateDto { Name = "", Location = "", NetworkAddress = "" };
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = await _controller.PutNode(1, dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── DELETE /api/nodes/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task DeleteNode_DeveRetornarNoContentQuandoDeletaComSucesso() {
        // Arrange
        _mockService.Setup(s => s.DeleteNodeAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteNode(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteNode_DeveRetornarNotFoundQuandoNoNaoExiste() {
        // Arrange
        _mockService.Setup(s => s.DeleteNodeAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteNode(99);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteNode_DeveRetornarConflictQuandoHaLinksAtivos() {
        // Arrange
        _mockService.Setup(s => s.DeleteNodeAsync(1)).ThrowsAsync(new DbUpdateException("FK violation", new Exception()));

        // Act
        var result = await _controller.DeleteNode(1);

        // Assert
        var conflict = result.Should().BeOfType<ConflictObjectResult>().Subject;
        conflict.StatusCode.Should().Be(409);
    }
}
