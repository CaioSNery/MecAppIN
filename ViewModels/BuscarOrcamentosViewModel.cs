using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;
using MecAppIN.Pdfs;
using MecAppIN.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;




namespace MecAppIN.ViewModels
{
    public class BuscarOrcamentosViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Orcamentos> Orcamentos { get; set; }

        private List<Orcamentos> _todosOrcamentos;

        private Orcamentos _orcamentoSelecionado;
        public Orcamentos OrcamentoSelecionado
        {
            get => _orcamentoSelecionado;
            set
            {
                _orcamentoSelecionado = value;
                AtualizarBotoes();
            }
        }

        // ===============================
        // BUSCA
        // ===============================
        private string _textoBuscaCliente;
        public string TextoBuscaCliente
        {
            get => _textoBuscaCliente;
            set
            {
                _textoBuscaCliente = value;
                PaginaAtual = 1;
                OnPropertyChanged();
                Filtrar();
            }
        }

        // ===============================
        // PAGINAÇÃO
        // ===============================
        private int _paginaAtual = 1;
        private const int TamanhoPagina = 50;

        public int PaginaAtual
        {
            get => _paginaAtual;
            set
            {
                if (_paginaAtual == value) return;
                _paginaAtual = value;
                OnPropertyChanged();
                Filtrar();
            }
        }

        private int _totalPaginas;
        public int TotalPaginas
        {
            get => _totalPaginas;
            set
            {
                _totalPaginas = value;
                OnPropertyChanged();
            }
        }

        // ===============================
        // COMMANDS
        // ===============================
        public ICommand EditarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand CriarOrdemServicoCommand { get; }
        public ICommand AbrirPdfCommand { get; }
        public ICommand ImprimirCommand { get; }
        public ICommand ProximaPaginaCommand { get; }
        public ICommand PaginaAnteriorCommand { get; }

        public BuscarOrcamentosViewModel()
        {
            Orcamentos = new ObservableCollection<Orcamentos>();
            Carregar();

            EditarCommand = new RelayCommand(Editar, PodeExecutar);
            ExcluirCommand = new RelayCommand(Excluir, PodeExecutar);
            CriarOrdemServicoCommand = new RelayCommand(ConfirmarCriacaoOs, PodeExecutar);
            AbrirPdfCommand = new RelayCommand(AbrirPdf, PodeExecutar);
            ImprimirCommand = new RelayCommand(ImprimirOrcamento, PodeExecutar);

            ProximaPaginaCommand = new RelayCommand(ProximaPagina);
            PaginaAnteriorCommand = new RelayCommand(PaginaAnterior);
        }

        private bool PodeExecutar() => OrcamentoSelecionado != null;

        private void AtualizarBotoes()
        {
            (EditarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ExcluirCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CriarOrdemServicoCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (AbrirPdfCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ImprimirCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ===============================
        // CARREGAR
        // ===============================
        private void Carregar()
        {
            using var db = new AppDbContext();

            _todosOrcamentos = db.Orcamentos
                .Include(o => o.Itens)
                .OrderByDescending(o => o.Data)
                .ToList();

            Filtrar();
        }

        // ===============================
        // FILTRAR + PAGINAR
        // ===============================
        private void Filtrar()
        {
            Orcamentos.Clear();

            var termo = TextoBuscaCliente?.Trim().ToLower();

            var filtrados = string.IsNullOrWhiteSpace(termo)
                ? _todosOrcamentos
                : _todosOrcamentos.Where(o =>
                    o.ClienteNome.ToLower().Contains(termo) ||
                    o.Veiculo.ToLower().Contains(termo) ||
                    o.Placa.ToLower().Contains(termo) ||
                    o.Id.ToString().Contains(termo)
                ).ToList();

            TotalPaginas = (int)Math.Ceiling(
                filtrados.Count / (double)TamanhoPagina
            );

            if (PaginaAtual > TotalPaginas && TotalPaginas > 0)
                PaginaAtual = TotalPaginas;

            var paginados = filtrados
                .Skip((PaginaAtual - 1) * TamanhoPagina)
                .Take(TamanhoPagina);

            foreach (var o in paginados)
                Orcamentos.Add(o);
        }

        // ===============================
        // PAGINAÇÃO ACTIONS
        // ===============================
        private void ProximaPagina()
        {
            if (PaginaAtual >= TotalPaginas)
                return;

            PaginaAtual++;
        }

        private void PaginaAnterior()
        {
            if (PaginaAtual <= 1)
                return;

            PaginaAtual--;
        }

        // ===============================
        // AÇÕES
        // ===============================
        private void Editar()
        {
            var mainVM = Application.Current.MainWindow.DataContext as MainViewModel;
            mainVM.TelaAtual = new OrcamentosViewModel(OrcamentoSelecionado.Id);
        }

        private void Excluir()
        {
            if (OrcamentoSelecionado == null)
                return;

            var resultado = MessageBox.Show(
                $"Deseja excluir o Orçamento #{OrcamentoSelecionado.Id} do cliente \"{OrcamentoSelecionado.ClienteNome}\"?\n\n" +
                "Esta ação também removerá o PDF salvo.",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {
                // Remove PDF
                var caminhoPdf = ObterCaminhoPdf(OrcamentoSelecionado);

                if (File.Exists(caminhoPdf))
                    File.Delete(caminhoPdf);

                // Remove do banco
                using var db = new AppDbContext();
                db.Orcamentos.Remove(OrcamentoSelecionado);
                db.SaveChanges();

                // Remove da lista local
                _todosOrcamentos.Remove(OrcamentoSelecionado);
                Filtrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao excluir o Orçamento:\n\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void ConfirmarCriacaoOs()
        {
            var resultado = MessageBox.Show(
                $"Deseja criar uma Ordem de Serviço a partir do Orçamento #{OrcamentoSelecionado.Id}?\n\n" +
                "O orçamento será removido após a criação da OS.",
                "Confirmar criação de OS",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (resultado != MessageBoxResult.Yes)
                return;

            CriarOrdemServico();
        }



        private void CriarOrdemServico()
        {
            var service = new OrcamentoService();
            service.ConverterEmOsEExcluir(OrcamentoSelecionado);

            _todosOrcamentos.Remove(OrcamentoSelecionado);
            Filtrar();

            MessageBox.Show(
                "Ordem de Serviço criada com sucesso!",
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }


        private void AbrirPdf()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = ObterCaminhoPdf(OrcamentoSelecionado),
                UseShellExecute = true
            });
        }

        private string ObterCaminhoPdf(Orcamentos o)
        {
            return Path.Combine(
                @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                "PDFs",
                "Orcamentos",
                o.Data.Year.ToString(),
                o.Data.Month.ToString("D2"),
                $"ORCAMENTO_{o.Id}.pdf"
            );
        }

        private void ImprimirOrcamento()
        {
            if (OrcamentoSelecionado == null)
                return;

            var caminhoPdf = ObterCaminhoPdf(OrcamentoSelecionado);

            PdfService.ImprimirPdfSeguro(caminhoPdf);

        }

       


        // ===============================
        // PROPERTY CHANGED
        // ===============================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
