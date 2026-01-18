namespace MecAppIN.Models
{
    public class Orcamentos
    {
        public int Id { get; set; }
    public DateTime Data { get; set; }
    public decimal Total { get; set; }

    public int? ClienteId { get; set; }
    public Clientes Cliente { get; set; }

    public string ClienteNome { get; set; } 
    public string Veiculo { get; set; }
    public string Placa { get; set; }

    public string NomeClienteExibicao
{
    get
    {
        if (Cliente != null)
            return Cliente.Nome;

        return ClienteNome;
    }
}


    public List<ItemOrcamento> Itens { get; set; } = new();
    }
}
