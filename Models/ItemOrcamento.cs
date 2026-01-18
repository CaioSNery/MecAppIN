namespace MecAppIN.Models
{
    public class ItemOrcamento
    {
        public int Id { get; set; }

        public int OrcamentoId { get; set; }
        public Orcamentos Orcamento { get; set; }

        public string Servico { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
    }
}
