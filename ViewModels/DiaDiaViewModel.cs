using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Enums;
using MecAppIN.Models;
using MecAppIN.Pdf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MecAppIN.ViewModels
{
    public class DiaDiaViewModel : INotifyPropertyChanged
    {
        public string Titulo => "Financeiro - Movimento do Dia";

          // 👉 LISTAS PARA O COMBOBOX
        public Array TiposPagamento => Enum.GetValues(typeof(ETipoPagamento));
        public Array FormasPagamento => Enum.GetValues(typeof(ETipoFormaDePagamento));

        // bindings já existentes
        

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
            var lancamento = new LancamentoFinanceiro
            {
                Data = DateTime.Now,
                Valor = Valor,
                Tipo = Tipo,
                Forma = Forma
            };

            using var db = new AppDbContext();
            db.Lancamentos.Add(lancamento);
            db.SaveChanges();

            Lancamentos.Add(lancamento);

            Valor = 0;
            OnPropertyChanged(nameof(Valor));

            AtualizarTotais();
        }

        // =====================
        // TOTAIS (estilo Excel)
        // =====================
        public decimal TotalEntradas =>
            Lancamentos.Where(l => l.Tipo == ETipoPagamento.Entrada).Sum(l => l.Valor);

        public decimal TotalSaidas =>
            Lancamentos.Where(l => l.Tipo == ETipoPagamento.Saida).Sum(l => l.Valor);

        public decimal TotalDinheiro =>
            Lancamentos.Where(l => l.Forma == ETipoFormaDePagamento.Dinheiro).Sum(l => l.Valor);

        public decimal TotalCartao =>
            Lancamentos.Where(l => l.Forma == ETipoFormaDePagamento.Cartao).Sum(l => l.Valor);

        public decimal TotalFinal => TotalEntradas - TotalSaidas;

        private void AtualizarTotais()
        {
            OnPropertyChanged(nameof(TotalEntradas));
            OnPropertyChanged(nameof(TotalSaidas));
            OnPropertyChanged(nameof(TotalDinheiro));
            OnPropertyChanged(nameof(TotalCartao));
            OnPropertyChanged(nameof(TotalFinal));
        }

        // =====================
        // FECHAR DIA (PDF)
        // =====================
        private void FecharDia()
        {
            PdfFinanceiroService.GerarPdfDiario(DateTime.Today, Lancamentos.ToList());
        }
    }
}
