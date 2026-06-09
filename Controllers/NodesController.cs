using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronosDotnetApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NodesController : ControllerBase {

    private readonly INodeService _nodeService;

    public NodesController(INodeService nodeService) {
        _nodeService = nodeService;
    }

    /// <summary>
    /// Lista todos os nós ativos na rede DTN com seus saldos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<NodeDto>), 200)]
    public async Task<ActionResult<PagedResultDto<NodeDto>>> GetNodes([FromQuery] int page = 1, [FromQuery] int pageSize = 20) {
        if (page < 1 || pageSize < 1 || pageSize > 100) {
            return BadRequest(new { message = "Parâmetros de paginação inválidos. page >= 1, pageSize entre 1 e 100." });
        }
        var nodes = await _nodeService.GetAllNodesAsync(page, pageSize);
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
    [ProducesResponseType(409)]
    public async Task<ActionResult<NodeDto>> PostNode(NodeCreateDto dto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        try {
            var node = await _nodeService.CreateNodeAsync(dto);
            return CreatedAtAction(nameof(GetNode), new { id = node.IdNode }, node);
        } catch (DbUpdateException) {
            return Conflict(new { message = "Não foi possível criar o nó. Verifique se o endereço de rede já está em uso." });
        }
    }

    /// <summary>
    /// Atualiza as informações de um nó existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> PutNode(int id, NodeUpdateDto dto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        try {
            var updated = await _nodeService.UpdateNodeAsync(id, dto);
            if (!updated) {
                return NotFound(new { message = $"Nó com ID {id} não encontrado." });
            }
            return NoContent();
        } catch (DbUpdateException) {
            return Conflict(new { message = $"Não foi possível atualizar o nó com ID {id}. Verifique se o endereço de rede já está em uso." });
        }
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
