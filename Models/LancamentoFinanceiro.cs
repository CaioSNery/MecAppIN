

using MecAppIN.Enums;

namespace MecAppIN.Models
{
    public class LancamentoFinanceiro
    {
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public ETipoPagamento Tipo { get; set; } 
    public ETipoFormaDePagamento Forma { get; set; } 

    }
}