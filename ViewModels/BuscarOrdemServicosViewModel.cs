using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

        public BuscarOrdemServicosViewModel()
        {
            Ordens = new ObservableCollection<OrdemServicos>();
            Carregar();

            ReimprimirCommand = new RelayCommand(Reimprimir, PodeExecutar);
            ExcluirCommand = new RelayCommand(Excluir, PodeExecutar);
        }

        private bool PodeExecutar()
        {
            return OrdemSelecionada != null;
        }

        private void AtualizarBotoes()
        {
            (ReimprimirCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ExcluirCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                : _todasOrdens
                    .Where(o => o.ClienteNome.ToLower().Contains(termo))
                    .ToList();

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
                $"Deseja excluir a OS #{OrdemSelecionada.Id} do cliente \"{OrdemSelecionada.ClienteNome}\"?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            db.OrdemServicos.Remove(OrdemSelecionada);
            db.SaveChanges();

            Ordens.Remove(OrdemSelecionada);
        }

        // ===============================
        // REIMPRIMIR
        // ===============================
        private void Reimprimir()
        {
            if (OrdemSelecionada == null)
                return;

            MessageBox.Show(
                $"Reimprimir OS #{OrdemSelecionada.Id} (próximo passo)",
                "Reimpressão",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
