using MecAppIN.Models;

public class OrdemServicos
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public int? ClienteId { get; set; }
    public Clientes? Cliente { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string Veiculo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public List<ItemOrdemServico> Itens { get; set; } = new();
    public decimal Total { get; set; }
    public int? OrcamentoId { get; set; }
}
