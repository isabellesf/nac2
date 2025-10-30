using System;

namespace Domain
{
    public class ProdutoNaoEncontradoException : DomainException
    {
        public ProdutoNaoEncontradoException(string sku) : base($"Produto com SKU {sku} não encontrado.") { }
    }
}
