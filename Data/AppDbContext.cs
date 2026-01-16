

using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;

namespace MecAppIN.Data
{
    public class AppDbContext:DbContext
{
    public DbSet<Clientes>Clientes{get;set;}
    public DbSet<Orcamentos>Orçamentos{get;set;}
    public DbSet<OrdemServicos>OrdemServicos{get;set;}

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=oficina.db");
    }
}
}