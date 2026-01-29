
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using MecAppIN.Commands;
using MecAppIN.Models;

namespace MecAppIN.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // ===============================
        // NAVEGAÇÃO
        // ===============================
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
        public ICommand AbrirBuscarOrdemServicoCommand { get; }
        public ICommand AbrirDiaDiaCommand { get; }

        // ===============================
        // DATA / HORA
        // ===============================
        private string _dataHoraAtual;
        public string DataHoraAtual
        {
            get => _dataHoraAtual;
            set
            {
                _dataHoraAtual = value;
                OnPropertyChanged();
            }
        }

        private readonly DispatcherTimer _timer;

        // ===============================
        // CONSTRUTOR
        // ===============================
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

            
            AbrirBuscarOrdemServicoCommand = new RelayCommand(() =>
                TelaAtual = new BuscarOrdemServicosViewModel(this));

            AbrirDiaDiaCommand = new RelayCommand(() =>
                TelaAtual = new DiaDiaViewModel());

            // Tela inicial
            TelaAtual = new OrdemServicosViewModel();

            AtualizarHora();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += (s, e) => AtualizarHora();
            _timer.Start();
        }

        // ===============================
        // MÉTODOS AUXILIARES
        // ===============================

        private void AtualizarHora()
        {
            DataHoraAtual = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

}
