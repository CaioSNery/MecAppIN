using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MecAppIN.Models;

public class OrdemServicos : INotifyPropertyChanged
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;

    public int? ClienteId { get; set; }
    public Clientes Cliente { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteTelefone { get; set; } = string.Empty;

    public string ClienteEndereco { get; set; } = string.Empty;
    public string Veiculo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;

    public List<ItemOrdemServico> Itens { get; set; } = new();

    public string TipoMotor { get; set; } = string.Empty;

    
    private bool _pago;
    public bool Pago
    {
        get => _pago;
        set
        {
            if (_pago == value) return;
            _pago = value;
            OnPropertyChanged(nameof(Pago));
        }
    }
    public DateTime? DataPagamento { get; set; }
    
    [NotMapped]
    public bool PagoAnterior { get; set; }

    public decimal Total { get; set; }
    public int? OrcamentoId { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string prop)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
