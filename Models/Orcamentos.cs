using System.ComponentModel.DataAnnotations.Schema;

namespace MecAppIN.Models
{
    public class Orcamentos
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }

        public int? ClienteId { get; set; }
        public string ClienteNome { get; set; }

        public string Veiculo { get; set; }
        public string Placa { get; set; }

        public decimal Total { get; set; }

        public int NumeroOs { get; set; } = 0;

        public ICollection<ItemOrcamento> Itens { get; set; }

        
        [NotMapped]
        public string NumeroOsExibicao =>
            NumeroOs > 0 ? NumeroOs.ToString() : "--";
    }
}
