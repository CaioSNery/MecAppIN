using MecAppIN.Models;

public class OrdemServicos
{
    public int Id { get; set; }

    // ===============================
    // DATA DA ORDEM
    // ===============================
    public DateTime Data { get; set; } = DateTime.Now;

    // ===============================
    // CLIENTE (OPCIONAL)
    // ===============================
    public int? ClienteId { get; set; }
    public Clientes? Cliente { get; set; }

    // Snapshot do nome (histórico)
    public string ClienteNome { get; set; } = string.Empty;

    // ===============================
    // VEÍCULO
    // ===============================
    public string Veiculo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;

    // ===============================
    // ITENS / SERVIÇOS
    // ===============================
    public List<ItemOrdemServico> Itens { get; set; } = new();

    // ===============================
    // TOTAL DA OS
    // ===============================
    public decimal Total { get; set; }

    // ===============================
    // VÍNCULO COM ORÇAMENTO (OPCIONAL)
    // ===============================
    public int? OrcamentoId { get; set; }
}
