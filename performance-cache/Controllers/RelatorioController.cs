using Domain;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace performance_cache.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatorioController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;

        public RelatorioController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        [HttpGet("valor-total-estoque")]
        public async Task<ActionResult<decimal>> CalcularValorTotalEstoque()
        {
            var valorTotal = await _relatorioService.CalcularValorTotalEstoqueAsync();
            return Ok(valorTotal);
        }

        [HttpGet("abaixo-minimo")]
        public async Task<ActionResult<IEnumerable<Produto>>> ListarProdutosAbaixoEstoqueMinimo()
        {
            var produtos = await _relatorioService.ListarProdutosAbaixoEstoqueMinimoAsync();
            return Ok(produtos);
        }

        [HttpGet("vencendo-7-dias")]
        public async Task<ActionResult<IEnumerable<MovimentacaoEstoque>>> ListarProdutosVencendoEm7Dias()
        {
            var movimentacoes = await _relatorioService.ListarProdutosVencendoEm7DiasAsync();
            return Ok(movimentacoes);
        }
    }
}
