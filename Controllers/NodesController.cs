namespace ChronosDotnetApi.Controllers; // Namespace dos controladores de apresentacao da API.

using ChronosDotnetApi.Domain; // Importa as classes de entidade do dominio.
using ChronosDotnetApi.Infrastructure; // Importa o contexto do banco de dados.
using Microsoft.AspNetCore.Mvc; // Importa as definicoes de controladores e rotas HTTP do ASP.NET Core.
using Microsoft.EntityFrameworkCore; // Importa metodos assincronos e de carregamento do EF Core.

[ApiController] // Declara a classe como controlador de API, ativando comportamento automatico de DTOs.
[Route("api/[controller]")] // Define a rota de acesso base baseada no nome do controller (/api/nodes).
public class NodesController : ControllerBase { // Declaracao do controlador NodesController.

    private readonly ChronosDbContext _context; // Variavel interna para o contexto do banco de dados.

    public NodesController(ChronosDbContext context) { // Construtor recebendo injeção de dependência.
        _context = context; // Atribui a instancia de banco injetada.
    } // Fim do construtor.

    [HttpGet] // Atribui metodo GET HTTP para listagem de dados.
    public async Task<ActionResult<IEnumerable<Node>>> GetNodes() { // Assinatura assincrona de listagem.
        return await _context.Nodes // Acessa a colecao de nos cadastrados.
            .Include(n => n.Balances) // Carrega de forma otimizada os saldos de stablecoin associados (JOIN).
            .ToListAsync(); // Converte de forma assincrona a query em uma lista de retorno.
    } // Fim do metodo GetNodes.

    [HttpGet("{id}")] // Atribui metodo GET HTTP filtrando por parametro de rota.
    public async Task<ActionResult<Node>> GetNode(int id) { // Assinatura de busca individual.
        var node = await _context.Nodes // Consulta a tabela.
            .Include(n => n.Balances) // Carrega em conjunto os saldos do no.
            .FirstOrDefaultAsync(n => n.IdNode == id); // Retorna o primeiro que coincidir com o ID ou nulo.

        if (node == null) { // Condicional se nao encontrar o registro.
            return NotFound(); // Retorna codigo de status HTTP 404 Not Found.
        } // Fim do bloco IF.

        return node; // Retorna o objeto encontrado com status HTTP 200 OK.
    } // Fim do metodo GetNode.

    [HttpPost] // Atribui metodo POST HTTP para insercao.
    public async Task<ActionResult<Node>> PostNode(Node node) { // Assinatura de criacao.
        _context.Nodes.Add(node); // Enfileira a entidade no rastreador de alteracoes do contexto.
        await _context.SaveChangesAsync(); // Grava fisicamente as alteracoes no banco (COMMIT).

        return CreatedAtAction(nameof(GetNode), new { id = node.IdNode }, node); // Retorna 201 Created.
    } // Fim do metodo PostNode.

    [HttpDelete("{id}")] // Atribui metodo DELETE HTTP para remocao de registro.
    public async Task<IActionResult> DeleteNode(int id) { // Assinatura de exclusao.
        var node = await _context.Nodes.FindAsync(id); // Procura o no correspondente na base.
        if (node == null) { // Se nao localizar o registro.
            return NotFound(); // Retorna status HTTP 404.
        } // Fim do IF.

        _context.Nodes.Remove(node); // Remove a entidade do banco.
        await _context.SaveChangesAsync(); // Confirma as alteracoes fisicas no banco de dados.

        return NoContent(); // Retorna status HTTP 204 No Content indicando sucesso da operacao.
    } // Fim do metodo DeleteNode.
} // Fim da classe NodesController.
