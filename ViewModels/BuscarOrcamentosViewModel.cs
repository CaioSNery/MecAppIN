using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;
using MecAppIN.Pdf;
using MecAppIN.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Xps.Packaging;

namespace MecAppIN.ViewModels
{
    public class BuscarOrcamentosViewModel
    {
        public ObservableCollection<Orcamentos> Orcamentos { get; set; }

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

        private string _textoBuscaCliente;
        public string TextoBuscaCliente
        {
            get => _textoBuscaCliente;
            set
            {
                _textoBuscaCliente = value;
                Filtrar();
            }
        }

        private List<Orcamentos> _todosOrcamentos;

        // COMMANDS
        public ICommand EditarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand CriarOrdemServicoCommand { get; }
        public ICommand AbrirPdfCommand { get; }
        public ICommand ImprimirCommand { get; }


        public BuscarOrcamentosViewModel()
        {
            Orcamentos = new ObservableCollection<Orcamentos>();
            Carregar();

            EditarCommand = new RelayCommand(Editar, PodeExecutar);
            ExcluirCommand = new RelayCommand(Excluir, PodeExecutar);
            CriarOrdemServicoCommand = new RelayCommand(CriarOrdemServico, PodeExecutar);
            AbrirPdfCommand = new RelayCommand(AbrirPdf, PodeExecutar);
            ImprimirCommand = new RelayCommand(ImprimirOrcamento, PodeExecutar);
        }

        private bool PodeExecutar()
        {
            return OrcamentoSelecionado != null;
        }

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
        // FILTRO
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

            foreach (var o in filtrados)
                Orcamentos.Add(o);
        }

        // ===============================
        // EDITAR ORÇAMENTO
        // ===============================
        private void Editar()
        {
            if (OrcamentoSelecionado == null)
                return;

            var mainVM = Application.Current.MainWindow.DataContext as MainViewModel;
            if (mainVM == null)
                return;


            mainVM.TelaAtual = new OrcamentosViewModel(OrcamentoSelecionado);
        }





        // ===============================
        // EXCLUIR
        // ===============================
        private void Excluir()
        {
            if (OrcamentoSelecionado == null)
                return;

            var resultado = MessageBox.Show(
                $"Deseja excluir o orçamento do cliente \"{OrcamentoSelecionado.ClienteNome}\"?\n\n" +
                "O PDF também será removido.",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {
                var caminhoPdf = ObterCaminhoPdf(OrcamentoSelecionado);

                if (File.Exists(caminhoPdf))
                    File.Delete(caminhoPdf);

                using var db = new AppDbContext();
                db.Orcamentos.Remove(OrcamentoSelecionado);
                db.SaveChanges();

                Orcamentos.Remove(OrcamentoSelecionado);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao excluir orçamento:\n\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        // ===============================
        // ABRIR PDF DO ORÇAMENTO
        // ===============================
        private void AbrirPdf()
        {
            var caminhoPdf = Path.Combine(
                @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                "PDFs",
                "Orcamentos",
                OrcamentoSelecionado.Data.Year.ToString(),
                OrcamentoSelecionado.Data.Month.ToString("D2"),
                $"ORCAMENTO_{OrcamentoSelecionado.Id}.pdf"
            );

            if (!File.Exists(caminhoPdf))
            {
                MessageBox.Show(
                    "PDF do orçamento não encontrado.",
                    "Arquivo não encontrado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = caminhoPdf,
                UseShellExecute = true
            });
        }



        private void CriarOrdemServico()
        {
            if (OrcamentoSelecionado == null)
                return;

            try
            {
                var service = new OrcamentoService();
                service.ConverterEmOsEExcluir(OrcamentoSelecionado);

                Orcamentos.Remove(OrcamentoSelecionado);

                MessageBox.Show("OS criada e orçamento excluído com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

            if (!File.Exists(caminhoPdf))
            {
                MessageBox.Show("PDF do orçamento não encontrado.");
                return;
            }

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true)
                return;

            var tempXps = Path.Combine(
                Path.GetTempPath(),
                $"ORCAMENTO_{OrcamentoSelecionado.Id}.xps"
            );

            try
            {
                var pdf = new OrcamentoPdf(OrcamentoSelecionado);
                pdf.GenerateXps(tempXps);

                using var xpsDoc = new XpsDocument(tempXps, FileAccess.Read);
                var paginator = xpsDoc.GetFixedDocumentSequence().DocumentPaginator;

                printDialog.PrintDocument(
                    paginator,
                    $"Orçamento #{OrcamentoSelecionado.Id}"
                );
            }
            finally
            {
                if (File.Exists(tempXps))
                    File.Delete(tempXps);
            }
        }


    }
}
