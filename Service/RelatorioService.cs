using Domain;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IMovimentacaoRepository _movimentacaoRepository;

        public RelatorioService(IProdutoRepository produtoRepository, IMovimentacaoRepository movimentacaoRepository)
        {
            _produtoRepository = produtoRepository;
            _movimentacaoRepository = movimentacaoRepository;
        }

        public async Task<decimal> CalcularValorTotalEstoqueAsync()
        {
            // Regra de Negócio: Calcular valor total do estoque (quantidade × preço)
            var todosProdutos = await _produtoRepository.GetAllAsync();
            return todosProdutos.Sum(p => p.QuantidadeEmEstoque * p.PrecoUnitario);
        }

        public async Task<IEnumerable<Produto>> ListarProdutosAbaixoEstoqueMinimoAsync()
        {
            // Regra de Negócio: Identificar produtos com estoque abaixo do mínimo
            var todosProdutos = await _produtoRepository.GetAllAsync();
            return todosProdutos.Where(p => p.QuantidadeEmEstoque < p.QuantidadeMinimaEstoque).ToList();
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ListarProdutosVencendoEm7DiasAsync()
        {
            // Regra de Negócio: Listar produtos que vencerão em até 7 dias
            var dataLimite = DateTime.Today.AddDays(7);

            // Busca todas as movimentações de entrada (que contêm a DataValidade)
            // Filtra as movimentações de produtos perecíveis que vencem em até 7 dias
            // Nota: No Entity Framework Core In-Memory, a consulta pode ser limitada, mas para
            // um banco real, seria mais eficiente buscar apenas Movimentacoes de ENTRADA.
            // Como estamos usando In-Memory, vamos buscar todas as movimentações e filtrar na memória.

            var todasMovimentacoes = await _movimentacaoRepository.GetByProdutoSkuAsync(null); // Busca todas as movimentações (o null é um placeholder, pois a interface não tem um GetAll)

            // Como a interface IMovimentacaoRepository não tem um GetAll, vamos buscar os produtos
            // e depois buscar as movimentações de entrada para cada produto perecível.
            // Para simplificar no In-Memory, vamos criar um método GetAll no repositório.

            // ***********************************************************************************
            // ATENÇÃO: A interface IMovimentacaoRepository precisa de um método GetAllAsync() para esta funcionalidade.
            // Vamos adicionar este método e depois refatorar o código aqui.
            // ***********************************************************************************
            
            // Refatorando para usar apenas o repositório de produto para o relatório de estoque mínimo,
            // e assumindo que o repositório de movimentação será atualizado no próximo passo.

            // Por enquanto, vou implementar a lógica assumindo que terei acesso a todas as movimentações
            // que contêm DataValidade.

            // ***********************************************************************************
            var dataLimite = DateTime.Today.AddDays(7);

            // Busca todas as movimentações e filtra as que são de ENTRADA (pois contêm DataValidade)
            // e são de produtos perecíveis, com data de validade dentro dos próximos 7 dias.
            var todasMovimentacoes = await _movimentacaoRepository.GetAllAsync();

            return todasMovimentacoes
                .Where(m => m.Tipo == TipoMovimentacao.ENTRADA &&
                            m.Produto != null &&
                            m.Produto.Categoria == CategoriaProduto.PERECIVEL &&
                            m.DataValidade.HasValue &&
                            m.DataValidade.Value.Date >= DateTime.Today.Date &&
                            m.DataValidade.Value.Date <= dataLimite.Date)
                .ToList();
        }
    }
}
