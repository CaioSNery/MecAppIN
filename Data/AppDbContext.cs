

using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;

namespace MecAppIN.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Orcamentos> Orcamentos { get; set; }
        public DbSet<ItemOrcamento> ItensOrcamento { get; set; }

        public DbSet<OrdemServicos> OrdemServicos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=oficina.db");
        }
    }
}