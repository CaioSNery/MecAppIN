using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;
using MecAppIN.Services;
using MecAppIN.Views;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Xps.Packaging;

namespace MecAppIN.ViewModels
{
    public class BuscarOrdemServicosViewModel
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
                Filtrar();
            }
        }

        private List<OrdemServicos> _todasOrdens;

        // COMMANDS
        public ICommand ReimprimirCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand EditarCommand { get; }


        private readonly MainViewModel _mainVm;

        public BuscarOrdemServicosViewModel(MainViewModel mainVm)
        {
            _mainVm = mainVm;

            Ordens = new ObservableCollection<OrdemServicos>();
            Carregar();

            ReimprimirCommand = new RelayCommand(Reimprimir, PodeExecutar);
            ExcluirCommand = new RelayCommand(Excluir, PodeExecutar);
            EditarCommand = new RelayCommand(Editar, PodeExecutar);
        }

        private bool PodeExecutar()
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

            Filtrar();
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

            foreach (var os in filtradas)
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

                var caminhoPdf = ObterCaminhoPdf(OrdemSelecionada);

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

        private string ObterCaminhoPdf(OrdemServicos os)
        {
            return Path.Combine(
                @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                "PDFs",
                "OrdensDeServico",
                os.Data.Year.ToString(),
                os.Data.Month.ToString("D2"),
                $"OS_{os.Id}.pdf"
            );
        }

        private void Reimprimir()
        {
            if (OrdemSelecionada == null)
                return;

            var caminhoPdf = ObterCaminhoPdf(OrdemSelecionada);

            if (!File.Exists(caminhoPdf))
            {
                MessageBox.Show(
                    "O arquivo PDF desta OS não foi encontrado.\n" +
                    "Possivelmente ele foi removido ou movido.",
                    "PDF não encontrado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true)
                return;


            var tempXps = Path.Combine(
                Path.GetTempPath(),
                $"OS_{OrdemSelecionada.Id}.xps"
            );

            try
            {

                var pdf = new OrdemServicoPdf(OrdemSelecionada);
                pdf.GenerateXps(tempXps);

                using var xpsDoc = new XpsDocument(tempXps, FileAccess.Read);
                var paginator =
                    xpsDoc.GetFixedDocumentSequence().DocumentPaginator;

                printDialog.PrintDocument(
                    paginator,
                    $"Reimpressão OS #{OrdemSelecionada.Id}"
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
