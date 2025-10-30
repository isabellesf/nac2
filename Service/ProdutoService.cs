using Domain;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<Produto> CadastrarProdutoAsync(Produto produto)
        {
            // Regra de Negócio: Implementar validação de categoria vs dados obrigatórios
            // Para a entidade Produto, a única validação é se o SKU já existe.
            // A validação de lote/validade é feita na Movimentação.

            var produtoExistente = await _produtoRepository.GetBySkuAsync(produto.CodigoSKU);
            if (produtoExistente != null)
            {
                throw new ValidacaoException($"Produto com SKU {produto.CodigoSKU} já cadastrado.");
            }

            // Garante que o saldo inicial é 0
            produto.QuantidadeEmEstoque = 0;

            await _produtoRepository.AddAsync(produto);
            await _produtoRepository.SaveChangesAsync();
            return produto;
        }

        public async Task<Produto> GetProdutoBySkuAsync(string sku)
        {
            return await _produtoRepository.GetBySkuAsync(sku);
        }

        public async Task<IEnumerable<Produto>> ListarTodosProdutosAsync()
        {
            return await _produtoRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Produto>> ListarProdutosAbaixoEstoqueMinimoAsync()
        {
            // Regra de Negócio: Criar método para verificar produtos abaixo do estoque mínimo
            var todosProdutos = await _produtoRepository.GetAllAsync();
            return todosProdutos.Where(p => p.QuantidadeEmEstoque < p.QuantidadeMinimaEstoque).ToList();
        }
    }
}
