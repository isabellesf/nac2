using System;

namespace Domain
{
    public class EstoqueInsuficienteException : DomainException
    {
        public EstoqueInsuficienteException(string sku, int disponivel, int solicitado) 
            : base($"Estoque insuficiente para o produto {sku}. Disponível: {disponivel}, Solicitado: {solicitado}.") { }
    }
}
