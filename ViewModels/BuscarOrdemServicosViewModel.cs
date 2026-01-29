using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Enums;
using MecAppIN.Helpers;
using MecAppIN.Pdfs;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MecAppIN.ViewModels
{
    public class BuscarOrdemServicosViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<OrdemServicos> Ordens { get; set; }

        private OrdemServicos _ordemSelecionada;
        public OrdemServicos OrdemSelecionada
        {
            get => _ordemSelecionada;
            set
            {
                _ordemSelecionada = value;
                AtualizarBotoes();
            }
        }

        private string _textoBuscaCliente;
        public string TextoBuscaCliente
        {
            get => _textoBuscaCliente;
            set
            {
                _textoBuscaCliente = value;
                _paginaAtual = 1;
                Filtrar();
            }
        }

        private EFiltroPagamento _filtroPagamento = EFiltroPagamento.Todas;
        public EFiltroPagamento FiltroPagamento
        {
            get => _filtroPagamento;
            set
            {
                if (_filtroPagamento == value)
                    return;

                _filtroPagamento = value;
                PaginaAtual = 1;
                Filtrar();
            }
        }



        private int _paginaAtual = 1;
        private const int TamanhoPagina = 40;

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




        private List<OrdemServicos> _todasOrdens;

        // COMMANDS
        public ICommand ReimprimirCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand ProximaPaginaCommand { get; }
        public ICommand PaginaAnteriorCommand { get; }
        public ICommand MostrarPagasCommand { get; }
        public ICommand MostrarNaoPagasCommand { get; }
        public ICommand MostrarTodasCommand { get; }




        private readonly MainViewModel _mainVm;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public BuscarOrdemServicosViewModel(MainViewModel mainVm)
        {
            _mainVm = mainVm;

            Ordens = new ObservableCollection<OrdemServicos>();
            Carregar();

            ReimprimirCommand = new RelayCommand(Reimprimir, PodeReimprimir);
            ExcluirCommand = new RelayCommand(Excluir, PodeEditarOuExcluir);
            EditarCommand = new RelayCommand(Editar, PodeEditarOuExcluir);
            ProximaPaginaCommand = new RelayCommand(ProximaPagina);
            PaginaAnteriorCommand = new RelayCommand(PaginaAnterior);
            MostrarPagasCommand = new RelayCommand(() =>
            {
                PaginaAtual = 1;
                FiltroPagamento = EFiltroPagamento.Pagas;
            });

            MostrarNaoPagasCommand = new RelayCommand(() =>
            {
                PaginaAtual = 1;
                FiltroPagamento = EFiltroPagamento.NaoPagas;
            });

            MostrarTodasCommand = new RelayCommand(() =>
            {
                PaginaAtual = 1;
                FiltroPagamento = EFiltroPagamento.Todas;
            });


        }

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



        private bool PodeEditarOuExcluir()
        {
            return OrdemSelecionada != null && !OrdemSelecionada.Pago;
        }

        private bool PodeReimprimir()
        {
            return OrdemSelecionada != null;
        }


        private void AtualizarBotoes()
        {
            (ReimprimirCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ExcluirCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (EditarCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ===============================
        // CARREGAR
        // ===============================
        private void Carregar()
        {
            using var db = new AppDbContext();

            _todasOrdens = db.OrdemServicos
    .Include(o => o.Itens)
    .OrderByDescending(o => o.Data)
    .ToList();

            RegistrarEventos(_todasOrdens);
            Filtrar();

        }

        private void RegistrarEventos(IEnumerable<OrdemServicos> ordens)
        {
            foreach (var os in ordens)
            {
                os.PagoAnterior = os.Pago;
                os.PropertyChanged -= Ordem_PropertyChanged;
                os.PropertyChanged += Ordem_PropertyChanged;
            }
        }



        private void Ordem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(OrdemServicos.Pago))
                return;

            var os = sender as OrdemServicos;
            if (os == null)
                return;

            // ===============================
            // MARCAR COMO PAGA
            // ===============================
            if (os.Pago && !os.PagoAnterior)
            {
                var resultado = MessageBox.Show(
                    $"Deseja marcar a OS #{os.Id} como PAGA?",
                    "Confirmar pagamento",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (resultado != MessageBoxResult.Yes)
                {
                    os.Pago = os.PagoAnterior;
                    return;
                }

                os.DataPagamento = DateTime.Now;
            }

            // ===============================
            // ESTORNAR PAGAMENTO
            // ===============================
            if (!os.Pago && os.PagoAnterior)
            {
                var resultado = MessageBox.Show(
                    $"Deseja ESTORNAR o pagamento da OS #{os.Id}?",
                    "Confirmar estorno",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (resultado != MessageBoxResult.Yes)
                {
                    os.Pago = os.PagoAnterior;
                    return;
                }

                os.DataPagamento = null; //  LIMPA DATA
            }

            SalvarPagoNoBanco(os);

            os.PagoAnterior = os.Pago;

            PaginaAtual = 1;
            Filtrar();
            AtualizarBotoes();
        }





        private void SalvarPagoNoBanco(OrdemServicos os)
        {
            using var db = new AppDbContext();

            var entidade = db.OrdemServicos
                .Include(o => o.Itens)
                .First(o => o.Id == os.Id);

            entidade.Pago = os.Pago;
            entidade.DataPagamento = os.DataPagamento;

            db.SaveChanges();

            //  REGERA O PDF COM STATUS ATUALIZADO
            GerarPdfOsAtualizado(entidade);

            _todasOrdens = db.OrdemServicos
                .Include(o => o.Itens)
                .OrderByDescending(o => o.Data)
                .ToList();

            RegistrarEventos(_todasOrdens);
        }

        private void GerarPdfOsAtualizado(OrdemServicos os)
        {
            var caminho = PdfPathHelper.ObterCaminhoOs(os);

            // garante pasta
            Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);

            // apaga o PDF antigo
            if (File.Exists(caminho))
                File.Delete(caminho);

            // gera novamente (agora com OS.Pago = true)
            var pdf = new OrdemServicoPdf(os);
            pdf.GeneratePdf(caminho);
        }







        // ===============================
        // FILTRAR (DINÂMICO)
        // ===============================
        private void Filtrar()
        {
            Ordens.Clear();

            var termo = TextoBuscaCliente?.Trim().ToLower();

            var filtradas = string.IsNullOrWhiteSpace(termo)
                ? _todasOrdens
                : _todasOrdens.Where(o =>
                    o.ClienteNome.ToLower().Contains(termo) ||
                    o.Veiculo.ToLower().Contains(termo) ||
                    o.Placa.ToLower().Contains(termo) ||
                    o.Id.ToString().Contains(termo)
                ).ToList();
            if (FiltroPagamento == EFiltroPagamento.Pagas)
            {
                filtradas = filtradas.Where(o => o.Pago).ToList();
            }
            else if (FiltroPagamento == EFiltroPagamento.NaoPagas)
            {
                filtradas = filtradas.Where(o => !o.Pago).ToList();
            }


            TotalPaginas = (int)Math.Ceiling(
            filtradas.Count / (double)TamanhoPagina
            );

            if (PaginaAtual > TotalPaginas && TotalPaginas > 0)
                PaginaAtual = TotalPaginas;

            var paginadas = filtradas
                .Skip((PaginaAtual - 1) * TamanhoPagina)
                .Take(TamanhoPagina)
                .ToList();

            Ordens.Clear();
            foreach (var os in paginadas)
                Ordens.Add(os);

        }




        // ===============================
        // EXCLUIR
        // ===============================
        private void Excluir()
        {
            if (OrdemSelecionada == null)
                return;

            var resultado = MessageBox.Show(
                $"Deseja excluir a OS #{OrdemSelecionada.Id} do cliente \"{OrdemSelecionada.ClienteNome}\"?\n\n" +
                "Esta ação também removerá o PDF salvo.",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {

                var caminhoPdf = PdfPathHelper.ObterCaminhoOs(OrdemSelecionada);

                if (File.Exists(caminhoPdf))
                {
                    File.Delete(caminhoPdf);
                }


                using var db = new AppDbContext();
                db.OrdemServicos.Remove(OrdemSelecionada);
                db.SaveChanges();


                Ordens.Remove(OrdemSelecionada);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao excluir a Ordem de Serviço:\n\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        private void Editar()
        {
            if (OrdemSelecionada == null)
                return;

            _mainVm.TelaAtual = new OrdemServicosViewModel(OrdemSelecionada.Id);
        }



        // ===============================
        // REIMPRIMIR
        // ===============================



        private void Reimprimir()
        {
            if (OrdemSelecionada == null)
                return;

            var caminhoPdf = PdfPathHelper.ObterCaminhoOs(OrdemSelecionada);

            if (!File.Exists(caminhoPdf))
            {
                MessageBox.Show(
                    "PDF não encontrado para esta OS.",
                    "PDF",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            PdfService.ImprimirPdfSeguro(caminhoPdf);
        }



    }
}
