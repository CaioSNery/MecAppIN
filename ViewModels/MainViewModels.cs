using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MecAppIN.Commands;


namespace MecAppIN.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _telaAtual;
        public object TelaAtual
        {
            get => _telaAtual;
            set
            {
                _telaAtual = value;
                OnPropertyChanged();
            }
        }

        public ICommand AbrirClientesCommand { get; }
        public ICommand AbrirOrcamentosCommand { get; }
        public ICommand AbrirOrdemServicoCommand { get; }
        public ICommand AbrirBuscarOrcamentosCommand { get; }


        public MainViewModel()
        {
            AbrirClientesCommand = new RelayCommand(() =>
                TelaAtual = new ClientesViewModel());

            AbrirOrcamentosCommand = new RelayCommand(() =>
                TelaAtual = new OrcamentosViewModel());

            AbrirOrdemServicoCommand = new RelayCommand(() =>
                TelaAtual = new OrdemServicosViewModel());
                
            AbrirBuscarOrcamentosCommand = new RelayCommand(() =>
                TelaAtual = new BuscarOrcamentosViewModel());


            // Tela inicial
            TelaAtual = new OrdemServicosViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}

