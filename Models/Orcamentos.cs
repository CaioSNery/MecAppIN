namespace MecAppIN.Models
{
    public class Orcamentos
    {
        public int Id { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        public string Descricao { get; set; } = string.Empty;

        public decimal ValorEstimado { get; set; }

        // Relacionamento com Cliente
        public int ClienteId { get; set; }
        public Clientes Cliente { get; set; }
    }
}
