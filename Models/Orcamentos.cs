namespace MecAppIN.Models
{
    public class Orcamentos
    {
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;

        public int ClienteId { get; set; }
        public Clientes Cliente { get; set; }

        public string Veiculo { get; set; }
        public string Placa { get; set; }

        public decimal Total { get; set; }

        public ICollection<ItemOrcamento> Itens { get; set; }
    }
}
