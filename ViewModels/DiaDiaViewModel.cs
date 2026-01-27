using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Enums;
using MecAppIN.Models;
using MecAppIN.Pdf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MecAppIN.ViewModels
{
    public class DiaDiaViewModel : INotifyPropertyChanged
    {
        public string Titulo => "Financeiro - Movimento do Dia";


        public Array TiposPagamento => Enum.GetValues(typeof(ETipoPagamento));
        public Array FormasPagamento => Enum.GetValues(typeof(ETipoFormaDePagamento));




        public ObservableCollection<LancamentoFinanceiro> Lancamentos { get; set; }
            = new ObservableCollection<LancamentoFinanceiro>();

        public decimal Valor { get; set; }
        public ETipoPagamento Tipo { get; set; }
        public ETipoFormaDePagamento Forma { get; set; }

        public ICommand AdicionarCommand { get; }
        public ICommand FecharDiaCommand { get; }

        public DiaDiaViewModel()
        {
            AdicionarCommand = new RelayCommand(AdicionarLancamento);
            FecharDiaCommand = new RelayCommand(FecharDia);

            CarregarLancamentosDoDia();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private string _descricao;
        public string Descricao
        {
            get => _descricao;
            set
            {
                _descricao = value;
                OnPropertyChanged();
            }
        }


        private void CarregarLancamentosDoDia()
        {
            using var db = new AppDbContext();
            var hoje = DateTime.Today;

            var dados = db.Lancamentos
                .Where(l => l.Data.Date == hoje)
                .OrderBy(l => l.Data)
                .ToList();

            Lancamentos = new ObservableCollection<LancamentoFinanceiro>(dados);
            OnPropertyChanged(nameof(Lancamentos));
            AtualizarTotais();
        }

        private void AdicionarLancamento()
        {
            if (Valor <= 0)
            {
                MessageBox.Show(
                    "O valor precisa ser maior que zero.",
                    "Atenção",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(Descricao))
            {
                MessageBox.Show(
                    "A descrição é obrigatória.",
                    "Atenção",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var lancamento = new LancamentoFinanceiro
            {
                Data = DateTime.Now,
                Valor = Valor,
                Tipo = Tipo,
                Forma = Forma,
                Descricao = Descricao.Trim()
            };

            using var db = new AppDbContext();
            db.Lancamentos.Add(lancamento);
            db.SaveChanges();

            Lancamentos.Add(lancamento);

            Valor = 0;
            Descricao = string.Empty;

            OnPropertyChanged(nameof(Valor));
            OnPropertyChanged(nameof(Descricao));

            AtualizarTotais();
        }



        // =====================
        // TOTAIS 
        // =====================
        public decimal TotalEntradas =>
            Lancamentos.Where(l => l.Tipo == ETipoPagamento.Entrada).Sum(l => l.Valor);

        public decimal TotalSaidas =>
            Lancamentos.Where(l => l.Tipo == ETipoPagamento.Saida).Sum(l => l.Valor);

        public decimal TotalDinheiro =>
            Lancamentos.Where(l => l.Forma == ETipoFormaDePagamento.Dinheiro).Sum(l => l.Valor);

        public decimal TotalCartao =>
            Lancamentos.Where(l => l.Forma == ETipoFormaDePagamento.Cartao).Sum(l => l.Valor);

        public IEnumerable<LancamentoFinanceiro> Entradas =>
Lancamentos.Where(l => l.Tipo == ETipoPagamento.Entrada);

        public IEnumerable<LancamentoFinanceiro> Saidas =>
            Lancamentos.Where(l => l.Tipo == ETipoPagamento.Saida);


        public decimal TotalFinal => TotalEntradas - TotalSaidas;

        public void AtualizarTotais()
        {
            OnPropertyChanged(nameof(TotalEntradas));
            OnPropertyChanged(nameof(TotalSaidas));
            OnPropertyChanged(nameof(TotalDinheiro));
            OnPropertyChanged(nameof(TotalCartao));
            OnPropertyChanged(nameof(TotalFinal));
            OnPropertyChanged(nameof(Entradas));
            OnPropertyChanged(nameof(Saidas));

        }

        // =====================
        // FECHAR DIA (PDF)
        // =====================

        private void FecharDia()
        {
            PdfFinanceiroService.GerarPdfDiario(DateTime.Today, Lancamentos.ToList());

            MessageBox.Show(
                "Financeiro do dia salvo com sucesso!\nO PDF foi gerado.",
                "Financeiro",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
