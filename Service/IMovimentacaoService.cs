using Domain;
using System.Threading.Tasks;

namespace Service
{
    public interface IMovimentacaoService
    {
        Task<MovimentacaoEstoque> RegistrarEntradaAsync(string produtoSku, MovimentacaoEstoque movimentacao);
        Task<MovimentacaoEstoque> RegistrarSaidaAsync(string produtoSku, MovimentacaoEstoque movimentacao);
    }
}
