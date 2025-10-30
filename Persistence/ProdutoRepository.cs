using Domain;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly EstoqueContext _context;

        public ProdutoRepository(EstoqueContext context)
        {
            _context = context;
        }

        public async Task<Produto> GetBySkuAsync(string sku)
        {
            return await _context.Produtos
                .Include(p => p.Movimentacoes)
                .FirstOrDefaultAsync(p => p.CodigoSKU == sku);
        }

        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task AddAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
        }

        public Task UpdateAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Produto produto)
        {
            _context.Produtos.Remove(produto);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
