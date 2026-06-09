using ChronosDotnetApi.DTOs;
using ChronosDotnetApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronosDotnetApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AssetBalancesController : ControllerBase {

    private readonly IAssetBalanceService _service;

    public AssetBalancesController(IAssetBalanceService service) {
        _service = service;
    }

    /// <summary>
    /// Lista saldos de ativos com paginação. Filtra por nó via query param opcional.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<AssetBalanceDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PagedResultDto<AssetBalanceDto>>> GetBalances(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? nodeId = null) {
        if (page < 1 || pageSize < 1 || pageSize > 100) {
            return BadRequest(new { message = "Parâmetros de paginação inválidos. page >= 1, pageSize entre 1 e 100." });
        }
        var result = await _service.GetAllAsync(page, pageSize, nodeId);
        return Ok(result);
    }

    /// <summary>
    /// Busca um saldo de ativo por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AssetBalanceDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AssetBalanceDto>> GetBalance(int id) {
        var balance = await _service.GetByIdAsync(id);
        if (balance == null) {
            return NotFound(new { message = $"Saldo com ID {id} não encontrado." });
        }
        return Ok(balance);
    }

    /// <summary>
    /// Registra um novo saldo de stablecoin para um nó da rede.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AssetBalanceDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AssetBalanceDto>> PostBalance(AssetBalanceCreateDto dto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        try {
            var balance = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetBalance), new { id = balance.IdAsset }, balance);
        } catch (KeyNotFoundException ex) {
            return NotFound(new { message = ex.Message });
        } catch (DbUpdateException) {
            return BadRequest(new { message = "Não foi possível criar o saldo. Verifique os dados informados." });
        }
    }

    /// <summary>
    /// Atualiza o símbolo e o saldo de um ativo existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PutBalance(int id, AssetBalanceUpdateDto dto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        try {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) {
                return NotFound(new { message = $"Saldo com ID {id} não encontrado." });
            }
            return NoContent();
        } catch (DbUpdateException) {
            return BadRequest(new { message = $"Não foi possível atualizar o saldo com ID {id}." });
        }
    }

    /// <summary>
    /// Remove um saldo de ativo da rede.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteBalance(int id) {
        try {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) {
                return NotFound(new { message = $"Saldo com ID {id} não encontrado." });
            }
            return NoContent();
        } catch (DbUpdateException) {
            return BadRequest(new { message = $"Não foi possível remover o saldo com ID {id}." });
        }
    }
}
