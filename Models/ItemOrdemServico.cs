
namespace MecAppIN.Models
{
    public class ItemOrdemServico
{
    public int Id { get; set; }

    public int OrdemServicoId { get; set; }
    public OrdemServicos OrdemServico { get; set; }

    public string Servico { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}

}