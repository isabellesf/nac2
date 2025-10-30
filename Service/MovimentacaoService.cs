using Domain;
using Repository;
using System;
using System.Threading.Tasks;

namespace Service
{
    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IMovimentacaoRepository _movimentacaoRepository;

        public MovimentacaoService(IProdutoRepository produtoRepository, IMovimentacaoRepository movimentacaoRepository)
        {
            _produtoRepository = produtoRepository;
            _movimentacaoRepository = movimentacaoRepository;
        }

        public async Task<MovimentacaoEstoque> RegistrarEntradaAsync(string produtoSku, MovimentacaoEstoque movimentacao)
        {
            var produto = await _produtoRepository.GetBySkuAsync(produtoSku);

            if (produto == null)
            {
                throw new ApplicationException($"Produto com SKU {produtoSku} não encontrado.");
            }

            // Regra de Negócio: Validar quantidade positiva
            if (movimentacao.Quantidade <= 0)
            {
                throw new ArgumentException("A quantidade de entrada deve ser positiva.");
            }

            // Regra de Negócio: Validar data de validade para perecíveis
            if (produto.Categoria == CategoriaProduto.PERECIVEL && !movimentacao.IsValidaParaProdutoPerecivel(produto.Categoria))
            {
                throw new ArgumentException("Produtos perecíveis requerem Lote e Data de Validade.");
            }

            // Atualizar Saldo do Produto
            produto.QuantidadeEmEstoque += movimentacao.Quantidade;
            movimentacao.Tipo = TipoMovimentacao.ENTRADA;
            movimentacao.ProdutoCodigoSKU = produtoSku;
            movimentacao.Produto = produto;

            // Salvar Movimentação e Atualizar Produto
            await _movimentacaoRepository.AddAsync(movimentacao);
            await _produtoRepository.UpdateAsync(produto);
            await _movimentacaoRepository.SaveChangesAsync(); // Salva ambos no mesmo contexto

            return movimentacao;
        }

        public async Task<MovimentacaoEstoque> RegistrarSaidaAsync(string produtoSku, MovimentacaoEstoque movimentacao)
        {
            var produto = await _produtoRepository.GetBySkuAsync(produtoSku);

            if (produto == null)
            {
                throw new ApplicationException($"Produto com SKU {produtoSku} não encontrado.");
            }

            // Regra de Negócio: Validar quantidade positiva
            if (movimentacao.Quantidade <= 0)
            {
                throw new ArgumentException("A quantidade de saída deve ser positiva.");
            }

            // Regra de Negócio: Verificar estoque suficiente para saídas
            if (produto.QuantidadeEmEstoque < movimentacao.Quantidade)
            {
                throw new InvalidOperationException($"Estoque insuficiente. Saldo atual: {produto.QuantidadeEmEstoque}, Saída solicitada: {movimentacao.Quantidade}.");
            }

            // Regra de Negócio: Validar data de validade para perecíveis (apenas se for relevante para rastreio)
            // Para a saída, a validação principal é o estoque. A validação de lote/validade é mais crítica na ENTRADA.
            // Para simplificar a Etapa 2, focaremos na validação de entrada.

            // Atualizar Saldo do Produto
            produto.QuantidadeEmEstoque -= movimentacao.Quantidade;
            movimentacao.Tipo = TipoMovimentacao.SAIDA;
            movimentacao.ProdutoCodigoSKU = produtoSku;
            movimentacao.Produto = produto;

            // Salvar Movimentação e Atualizar Produto
            await _movimentacaoRepository.AddAsync(movimentacao);
            await _produtoRepository.UpdateAsync(produto);
            await _movimentacaoRepository.SaveChangesAsync(); // Salva ambos no mesmo contexto

            return movimentacao;
        }
    }
}
