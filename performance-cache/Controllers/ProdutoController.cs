using Domain;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace performance_cache.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> CadastrarProduto([FromBody] Produto produto)
        {
            var novoProduto = await _produtoService.CadastrarProdutoAsync(produto);
            return CreatedAtAction(nameof(GetProdutoBySku), new { sku = novoProduto.CodigoSKU }, novoProduto);
        }

        [HttpGet("{sku}")]
        public async Task<ActionResult<Produto>> GetProdutoBySku(string sku)
        {
            var produto = await _produtoService.GetProdutoBySkuAsync(sku);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> ListarTodosProdutos()
        {
            var produtos = await _produtoService.ListarTodosProdutosAsync();
            return Ok(produtos);
        }

        [HttpGet("estoque-minimo")]
        public async Task<ActionResult<IEnumerable<Produto>>> ListarProdutosAbaixoEstoqueMinimo()
        {
            var produtos = await _produtoService.ListarProdutosAbaixoEstoqueMinimoAsync();
            return Ok(produtos);
        }
    }
}
