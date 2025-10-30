using Domain;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

namespace performance_cache.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentacaoController : ControllerBase
    {
        private readonly IMovimentacaoService _movimentacaoService;

        public MovimentacaoController(IMovimentacaoService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService;
        }

        [HttpPost("entrada/{sku}")]
        public async Task<ActionResult<MovimentacaoEstoque>> RegistrarEntrada(string sku, [FromBody] MovimentacaoEstoque movimentacao)
        {
            var novaMovimentacao = await _movimentacaoService.RegistrarEntradaAsync(sku, movimentacao);
            return CreatedAtAction(null, novaMovimentacao);
        }

        [HttpPost("saida/{sku}")]
        public async Task<ActionResult<MovimentacaoEstoque>> RegistrarSaida(string sku, [FromBody] MovimentacaoEstoque movimentacao)
        {
            var novaMovimentacao = await _movimentacaoService.RegistrarSaidaAsync(sku, movimentacao);
            return CreatedAtAction(null, novaMovimentacao);
        }
    }
}
