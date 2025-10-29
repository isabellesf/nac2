using System;

namespace Domain
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string Lote { get; set; } // Para produtos perecíveis
        public DateTime? DataValidade { get; set; } // Para produtos perecíveis

        // Chave estrangeira para Produto
        public string ProdutoCodigoSKU { get; set; }
        public Produto Produto { get; set; }

        public MovimentacaoEstoque()
        {
            DataMovimentacao = DateTime.UtcNow;
        }

        // Regra de Negócio: Produtos perecíveis devem ter lote e data de validade
        public bool IsValidaParaProdutoPerecivel(CategoriaProduto categoria)
        {
            if (categoria == CategoriaProduto.PERECIVEL)
            {
                return !string.IsNullOrEmpty(Lote) && DataValidade.HasValue;
            }
            return true;
        }

        // Regra de Negócio: Não é permitido entrada/saída de quantidade negativa
        public bool IsQuantidadePositiva()
        {
            return Quantidade > 0;
        }
    }
}
