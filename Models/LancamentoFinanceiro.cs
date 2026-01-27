using MecAppIN.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MecAppIN.Models
{
    public class LancamentoFinanceiro : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public ETipoPagamento Tipo { get; set; }
        public ETipoFormaDePagamento Forma { get; set; }

        private string _descricao;
        public string Descricao
        {
            get => _descricao;
            set
            {
                _descricao = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
