using System;
using System.Collections.Generic;

namespace Domain
{
    public class Produto
    {
        public string CodigoSKU { get; set; } // Identificador único
        public string Nome { get; set; }
        public CategoriaProduto Categoria { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int QuantidadeMinimaEstoque { get; set; }
        public DateTime DataCriacao { get; set; }
        public int QuantidadeEmEstoque { get; set; } = 0; // Campo para rastrear o estoque atual

        // Regra de Negócio: Produtos perecíveis devem ter lote e data de validade na movimentação,
        // mas a entidade Produto em si não precisa desses campos, apenas a Movimentação.

        public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; }

        public Produto()
        {
            DataCriacao = DateTime.UtcNow;
            Movimentacoes = new List<MovimentacaoEstoque>();
        }
    }
}
