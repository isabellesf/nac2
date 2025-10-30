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
                throw new ProdutoNaoEncontradoException(produtoSku);
            }

            // Regra de Negócio: Validar quantidade positiva
            if (movimentacao.Quantidade <= 0)
            {
                throw new ValidacaoException("A quantidade de entrada deve ser positiva.");
            }

            // Regra de Negócio: Validar data de validade para perecíveis
            if (produto.Categoria == CategoriaProduto.PERECIVEL && !movimentacao.IsValidaParaProdutoPerecivel(produto.Categoria))
            {
                throw new ValidacaoException("Produtos perecíveis requerem Lote e Data de Validade.");
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
                throw new ProdutoNaoEncontradoException(produtoSku);
            }

            // Regra de Negócio: Validar quantidade positiva
            if (movimentacao.Quantidade <= 0)
            {
                throw new ValidacaoException("A quantidade de saída deve ser positiva.");
            }

            // Regra de Negócio: Verificar estoque suficiente para saídas
            if (produto.QuantidadeEmEstoque < movimentacao.Quantidade)
            {
                throw new EstoqueInsuficienteException(produtoSku, produto.QuantidadeEmEstoque, movimentacao.Quantidade);
            }

            // Regra de Negócio: Produto perecível não pode ter movimentação após data de validade
            if (produto.Categoria == CategoriaProduto.PERECIVEL)
            {
                // Para simplificar, vamos assumir que a movimentação de saída é de um lote que já venceu.
                // Em um sistema real, seria necessário rastrear o lote específico que está saindo.
                // Aqui, vamos apenas verificar se o produto tem alguma movimentação de entrada vencida.
                
                var movimentacoesVencidas = produto.Movimentacoes
                    .Where(m => m.Tipo == TipoMovimentacao.ENTRADA && 
                                m.DataValidade.HasValue && 
                                m.DataValidade.Value.Date < DateTime.Today.Date)
                    .ToList();
                
                if (movimentacoesVencidas.Any())
                {
                    throw new ValidacaoException($"Não é possível registrar saída: o produto {produtoSku} possui lotes vencidos em estoque.");
                }
            }

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
