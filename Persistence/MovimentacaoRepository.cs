using Domain;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence
{
    public class MovimentacaoRepository : IMovimentacaoRepository
    {
        private readonly EstoqueContext _context;

        public MovimentacaoRepository(EstoqueContext context)
        {
            _context = context;
        }

        public async Task<MovimentacaoEstoque> GetByIdAsync(int id)
        {
            return await _context.MovimentacoesEstoque.FindAsync(id);
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> GetByProdutoSkuAsync(string sku)
        {
            return await _context.MovimentacoesEstoque
                .Where(m => m.ProdutoCodigoSKU == sku)
                .OrderByDescending(m => m.DataMovimentacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> GetAllAsync()
        {
            return await _context.MovimentacoesEstoque
                .Include(m => m.Produto)
                .OrderByDescending(m => m.DataMovimentacao)
                .ToListAsync();
        }

        public async Task AddAsync(MovimentacaoEstoque movimentacao)
        {
            await _context.MovimentacoesEstoque.AddAsync(movimentacao);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
