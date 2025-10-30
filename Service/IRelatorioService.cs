using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public interface IRelatorioService
    {
        Task<decimal> CalcularValorTotalEstoqueAsync();
        Task<IEnumerable<Produto>> ListarProdutosAbaixoEstoqueMinimoAsync();
        Task<IEnumerable<MovimentacaoEstoque>> ListarProdutosVencendoEm7DiasAsync();
    }
}
