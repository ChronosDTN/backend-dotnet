using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronosDotnetApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NodeLinksController : ControllerBase {

    private readonly INodeLinkService _service;

    public NodeLinksController(INodeLinkService service) {
        _service = service;
    }

    /// <summary>
    /// Lista todas as rotas/enlaces da rede DTN com paginação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<NodeLinkDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PagedResultDto<NodeLinkDto>>> GetLinks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20) {
        if (page < 1 || pageSize < 1 || pageSize > 100) {
            return BadRequest(new { message = "Parâmetros de paginação inválidos. page >= 1, pageSize entre 1 e 100." });
        }
        var result = await _service.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Busca uma rota/enlace por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NodeLinkDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<NodeLinkDto>> GetLink(int id) {
        var link = await _service.GetByIdAsync(id);
        if (link == null) {
            return NotFound(new { message = $"Rota com ID {id} não encontrada na rede DTN." });
        }
        return Ok(link);
    }

    /// <summary>
    /// Cria um novo enlace de roteamento entre dois nós da rede cislunar.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NodeLinkDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<NodeLinkDto>> PostLink(NodeLinkCreateDto dto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        try {
            var link = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetLink), new { id = link.IdRoute }, link);
        } catch (KeyNotFoundException ex) {
            return NotFound(new { message = ex.Message });
        } catch (DbUpdateException) {
            return BadRequest(new { message = "Não foi possível criar a rota. Verifique os nós informados." });
        }
    }

    /// <summary>
    /// Atualiza o status e a largura de banda de um enlace existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PutLink(int id, NodeLinkUpdateDto dto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        try {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) {
                return NotFound(new { message = $"Rota com ID {id} não encontrada." });
            }
            return NoContent();
        } catch (DbUpdateException) {
            return BadRequest(new { message = $"Não foi possível atualizar a rota com ID {id}." });
        }
    }

    /// <summary>
    /// Remove um enlace de roteamento da topologia da rede.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteLink(int id) {
        try {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) {
                return NotFound(new { message = $"Rota com ID {id} não encontrada." });
            }
            return NoContent();
        } catch (DbUpdateException) {
            return BadRequest(new { message = $"Não foi possível remover a rota com ID {id}." });
        }
    }
}
