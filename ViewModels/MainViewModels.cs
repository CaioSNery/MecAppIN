using System;
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
        // NAVEGAÇÃO DE TELAS
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
        // CONSTRUTOR ÚNICO
        // ===============================
        public MainViewModel()
        {
            // Commands de navegação
            AbrirClientesCommand = new RelayCommand(() =>
                TelaAtual = new ClientesViewModel());

            AbrirOrcamentosCommand = new RelayCommand(() =>
                TelaAtual = new OrcamentosViewModel());

            AbrirOrdemServicoCommand = new RelayCommand(() =>
                TelaAtual = new OrdemServicosViewModel());

            AbrirBuscarOrcamentosCommand = new RelayCommand(() =>
                TelaAtual = new BuscarOrcamentosViewModel());

            // Tela inicial
            TelaAtual = new OrcamentosViewModel();


            // ===============================
            // TIMER DE DATA / HORA
            // ===============================
            AtualizarHora();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += (s, e) => AtualizarHora();
            _timer.Start();
        }

        // ===============================
        // EDIÇÃO DE ORÇAMENTO
        // ===============================
        public void AbrirEdicaoOrcamento(Orcamentos orcamento)
        {
            TelaAtual = new OrcamentosViewModel(orcamento);
        }

        private void AtualizarHora()
        {
            DataHoraAtual = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        // ===============================
        // INotifyPropertyChanged
        // ===============================
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
