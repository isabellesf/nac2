using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public interface IProdutoService
    {
        Task<Produto> CadastrarProdutoAsync(Produto produto);
        Task<Produto> GetProdutoBySkuAsync(string sku);
        Task<IEnumerable<Produto>> ListarTodosProdutosAsync();
        Task<IEnumerable<Produto>> ListarProdutosAbaixoEstoqueMinimoAsync();
    }
}
