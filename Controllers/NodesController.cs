namespace ChronosDotnetApi.Controllers;

using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NodesController : ControllerBase {

    private readonly INodeService _nodeService;

    public NodesController(INodeService nodeService) {
        _nodeService = nodeService;
    }

    /// <summary>
    /// Lista todos os nós ativos na rede DTN com seus saldos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NodeDto>), 200)]
    public async Task<ActionResult<IEnumerable<NodeDto>>> GetNodes() {
        var nodes = await _nodeService.GetAllNodesAsync();
        return Ok(nodes);
    }

    /// <summary>
    /// Busca um nó específico por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NodeDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<NodeDto>> GetNode(int id) {
        var node = await _nodeService.GetNodeByIdAsync(id);
        if (node == null) {
            return NotFound(new { message = $"Nó com ID {id} não encontrado na rede DTN." });
        }
        return Ok(node);
    }

    /// <summary>
    /// Registra um novo nó gateway na rede cislunar.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NodeDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<NodeDto>> PostNode(NodeCreateDto dto) {
        var node = await _nodeService.CreateNodeAsync(dto);
        return CreatedAtAction(nameof(GetNode), new { id = node.IdNode }, node);
    }

    /// <summary>
    /// Atualiza as informações de um nó existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PutNode(int id, NodeUpdateDto dto) {
        var updated = await _nodeService.UpdateNodeAsync(id, dto);
        if (!updated) {
            return NotFound(new { message = $"Nó com ID {id} não encontrado." });
        }
        return NoContent();
    }

    /// <summary>
    /// Remove um nó da rede (bloqueado se houver links ativos).
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> DeleteNode(int id) {
        try {
            var deleted = await _nodeService.DeleteNodeAsync(id);
            if (!deleted) {
                return NotFound(new { message = $"Nó com ID {id} não encontrado." });
            }
            return NoContent();
        } catch (DbUpdateException) {
            return Conflict(new { 
                message = "Não é possível deletar este nó pois existem links de topologia ativos. Remova os links primeiro." 
            });
        }
    }
}