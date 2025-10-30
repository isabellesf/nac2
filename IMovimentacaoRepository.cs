using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface IMovimentacaoRepository
    {
        Task<MovimentacaoEstoque> GetByIdAsync(int id);
        Task<IEnumerable<MovimentacaoEstoque>> GetByProdutoSkuAsync(string sku);
        Task<IEnumerable<MovimentacaoEstoque>> GetAllAsync();
        Task AddAsync(MovimentacaoEstoque movimentacao);
        Task<int> SaveChangesAsync();
    }
}
