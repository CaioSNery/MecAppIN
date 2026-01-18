using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
        public ICommand ImprimirCommand { get; }

        public BuscarOrcamentosViewModel()
        {
            Orcamentos = new ObservableCollection<Orcamentos>();
            Carregar();

            EditarCommand = new RelayCommand(Editar, PodeExecutar);
            ExcluirCommand = new RelayCommand(Excluir, PodeExecutar);
            CriarOrdemServicoCommand = new RelayCommand(CriarOrdemServico, PodeExecutar);
            ImprimirCommand = new RelayCommand(Imprimir, PodeExecutar);
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
            (ImprimirCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private void Carregar()
        {
            using var db = new AppDbContext();

            _todosOrcamentos = db.Orcamentos
                .Include(o => o.Cliente)
                .Include(o => o.Itens)
                .OrderByDescending(o => o.Data)
                .ToList();

            Filtrar();
        }
        private void Filtrar()
        {
            Orcamentos.Clear();

            var termo = TextoBuscaCliente?.Trim().ToLower();

            var filtrados = string.IsNullOrWhiteSpace(termo)
                ? _todosOrcamentos
                : _todosOrcamentos
                    .Where(o => o.ClienteNome.ToLower().Contains(termo))
                    .ToList();

            foreach (var o in filtrados)
                Orcamentos.Add(o);
        }



        // ===============================
        // EDITAR
        // ===============================
        private void Editar()
        {
            // Acessa o MainViewModel atual
            var mainVM = Application.Current.MainWindow.DataContext as MainViewModel;

            if (mainVM == null)
                return;

            mainVM.AbrirEdicaoOrcamento(OrcamentoSelecionado);
        }


        // ===============================
        // EXCLUIR
        // ===============================
        private void Excluir()
        {
            var resultado = MessageBox.Show(
               $"Deseja excluir o orçamento do cliente \"{OrcamentoSelecionado.NomeClienteExibicao}\"?",
            "Confirmação",
             MessageBoxButton.YesNo,
             MessageBoxImage.Warning);

            if (OrcamentoSelecionado == null)
              return;

            if (resultado != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            db.Orcamentos.Remove(OrcamentoSelecionado);
            db.SaveChanges();

            Orcamentos.Remove(OrcamentoSelecionado);
        }

        // ===============================
        // CRIAR ORDEM DE SERVIÇO
        // ===============================
        private void CriarOrdemServico()
        {
            MessageBox.Show("Criar Ordem de Serviço a partir do orçamento (próximo passo).");
        }

        // ===============================
        // IMPRIMIR ORÇAMENTO
        // ===============================
        private void Imprimir()
        {
            MessageBox.Show(
                $"Imprimir orçamento do cliente:\n\n{OrcamentoSelecionado.Cliente.Nome}",
                "Impressão",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // Próximo passo: gerar PDF ou usar PrintDialog
        }
    }
}
