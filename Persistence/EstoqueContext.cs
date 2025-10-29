using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class EstoqueContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }

        public EstoqueContext(DbContextOptions<EstoqueContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração da chave primária e da relação 1:N
            modelBuilder.Entity<Produto>()
                .HasKey(p => p.CodigoSKU);

            modelBuilder.Entity<Produto>()
                .HasMany(p => p.Movimentacoes)
                .WithOne(m => m.Produto)
                .HasForeignKey(m => m.ProdutoCodigoSKU);

            // Configuração da chave primária para MovimentacaoEstoque
            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasKey(m => m.Id);

            // Configurar o enum para ser armazenado como string
            modelBuilder.Entity<Produto>()
                .Property(p => p.Categoria)
                .HasConversion<string>();

            modelBuilder.Entity<MovimentacaoEstoque>()
                .Property(m => m.Tipo)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
