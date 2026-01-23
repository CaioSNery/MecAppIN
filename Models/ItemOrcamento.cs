using MecAppIN.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MecAppIN.Models
{
    public class ItemOrcamento : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public int OrcamentoId { get; set; }
        public Orcamentos Orcamento { get; set; }

        public EBlocoMotor Bloco { get; set; }
        public bool IsPeca { get; set; }

        private string _servico;
        public string Servico
        {
            get => _servico;
            set
            {
                _servico = value;
                OnPropertyChanged();
            }
        }

        private int _quantidade;
        public int Quantidade
        {
            get => _quantidade;
            set
            {
                _quantidade = value;
                OnPropertyChanged();
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
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
