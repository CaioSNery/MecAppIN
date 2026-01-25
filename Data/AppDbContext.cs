

using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;

namespace MecAppIN.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Orcamentos> Orcamentos { get; set; }
        public DbSet<LancamentoFinanceiro> Lancamentos { get; set; }
        public DbSet<ItemOrdemServico> ItensOrdensServicos { get; set; }
        public DbSet<OrdemServicos> OrdemServicos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=oficina.db");
        }
    }
}