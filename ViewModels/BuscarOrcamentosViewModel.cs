using MecAppIN.Data;
using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace MecAppIN.ViewModels
{
    public class BuscarOrcamentosViewModel
    {
        public ObservableCollection<Orcamentos> Orcamentos { get; set; }

        public BuscarOrcamentosViewModel()
        {
            Orcamentos = new ObservableCollection<Orcamentos>();
            Carregar();
        }

        private void Carregar()
        {
            using var db = new AppDbContext();

            var lista = db.Orcamentos
                .Include(o => o.Cliente)
                .OrderByDescending(o => o.Data)
                .Take(50)
                .ToList();

            Orcamentos.Clear();
            foreach (var o in lista)
                Orcamentos.Add(o);
        }
    }
}
