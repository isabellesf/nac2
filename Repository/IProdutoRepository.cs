using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface IProdutoRepository
    {
        Task<Produto> GetBySkuAsync(string sku);
        Task<IEnumerable<Produto>> GetAllAsync();
        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task RemoveAsync(Produto produto);
        Task<int> SaveChangesAsync();
    }
}
