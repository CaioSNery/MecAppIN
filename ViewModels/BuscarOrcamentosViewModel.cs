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

            var lista = db.Orcamentos
                .Include(o => o.Cliente)
                .Include(o => o.Itens)
                .OrderByDescending(o => o.Data)
                .Take(50)
                .ToList();

            Orcamentos.Clear();
            foreach (var o in lista)
                Orcamentos.Add(o);
        }

        // ===============================
        // EDITAR
        // ===============================
        private void Editar()
        {
            MessageBox.Show("Abrir orçamento para edição (próximo passo).");
        }

        // ===============================
        // EXCLUIR
        // ===============================
        private void Excluir()
        {
            var resultado = MessageBox.Show(
                $"Deseja excluir o orçamento do cliente \"{OrcamentoSelecionado.Cliente.Nome}\"?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

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
