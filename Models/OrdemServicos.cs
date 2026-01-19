using MecAppIN.Models;

public class OrdemServicos
{
    public int Id { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;

    // CLIENTE (opcional)
    public int? ClienteID { get; set; }
    public Clientes? Cliente { get; set; }
    public string ClienteNome { get; set; } = string.Empty;

    // VEÍCULO
    public string Veiculo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;

    // SERVIÇOS
    public List<ItemOrdemServico> Itens { get; set; } = new();

    // VALOR FINAL
    public decimal Total { get; set; }

    // VÍNCULO OPCIONAL
    public int? OrcamentoId { get; set; }
}
