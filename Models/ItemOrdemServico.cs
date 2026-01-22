using System.ComponentModel;
using System.Runtime.CompilerServices;
using MecAppIN.Enums;

public class ItemOrdemServico : INotifyPropertyChanged
{
    public int Id { get; set; }

    public bool IsPeca { get; set; }

    public bool ValorEditavel { get; set; }


    public int OrdemServicosId { get; set; }
    public OrdemServicos OrdemServicos { get; set; }

    public EBlocoMotor Bloco { get; set; }

    private string _servico;
    public string Servico
    {
        get => _servico;
        set { _servico = value; OnPropertyChanged(); }
    }

    private int _quantidade;
    public int Quantidade
    {
        get => _quantidade;
        set
        {
            _quantidade = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Total));
        }
    }

    private decimal _valorUnitario;
    public decimal ValorUnitario
    {
        get => _valorUnitario;
        set
        {
            _valorUnitario = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Total));
        }
    }

    public decimal Total => Quantidade * ValorUnitario;

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? prop = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
