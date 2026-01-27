using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Enums;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MecAppIN.ViewModels
{
    public class ConsultaFinanceiroViewModel : INotifyPropertyChanged
    {
        public DateTime DataInicio { get; set; } = DateTime.Today;
        public DateTime DataFim { get; set; } = DateTime.Today;

        private decimal _totalEntradas;
        public decimal TotalEntradas
        {
            get => _totalEntradas;
            set { _totalEntradas = value; OnPropertyChanged(); }
        }

        private decimal _totalSaidas;
        public decimal TotalSaidas
        {
            get => _totalSaidas;
            set { _totalSaidas = value; OnPropertyChanged(); }
        }

        public decimal Resultado => TotalEntradas - TotalSaidas;

        public ICommand ConsultarCommand { get; }
        public ICommand HojeCommand { get; }
        public ICommand SemanaCommand { get; }
        public ICommand MesCommand { get; }
        public ICommand AbrirPdfCommand { get; }


        public ConsultaFinanceiroViewModel()
        {
            ConsultarCommand = new RelayCommand(Calcular);
            HojeCommand = new RelayCommand(Hoje);
            SemanaCommand = new RelayCommand(Semana);
            MesCommand = new RelayCommand(Mes);
            AbrirPdfCommand = new RelayCommand(AbrirPdfDoDia);


            Calcular();
        }

        private void AbrirPdfDoDia()
        {
            var data = DataInicio.Date;

            var basePath = Path.Combine(
                @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                "PDFs",
                "Financeiro",
                data.Year.ToString(),
                data.ToString("MM-yyyy")
            );

            var arquivo = Path.Combine(basePath, $"{data:yyyy-MM-dd}.pdf");

            if (!File.Exists(arquivo))
            {
                MessageBox.Show(
                    "Não existe PDF para o dia selecionado.",
                    "PDF não encontrado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = arquivo,
                UseShellExecute = true
            });
        }


        private void Calcular()
        {
            using var db = new AppDbContext();

            var lancamentos = db.Lancamentos
                .Where(l => l.Data.Date >= DataInicio.Date &&
                            l.Data.Date <= DataFim.Date)
                .ToList();

            TotalEntradas = lancamentos
                .Where(l => l.Tipo == ETipoPagamento.Entrada)
                .Sum(l => l.Valor);

            TotalSaidas = lancamentos
                .Where(l => l.Tipo == ETipoPagamento.Saida)
                .Sum(l => l.Valor);

            OnPropertyChanged(nameof(Resultado));
        }

        private void Hoje()
        {
            DataInicio = DateTime.Today;
            DataFim = DateTime.Today;
            Calcular();
        }

        private void Semana()
        {
            var hoje = DateTime.Today;
            var inicio = hoje.AddDays(-(int)hoje.DayOfWeek);
            DataInicio = inicio;
            DataFim = inicio.AddDays(6);
            Calcular();
        }

        private void Mes()
        {
            var hoje = DateTime.Today;
            DataInicio = new DateTime(hoje.Year, hoje.Month, 1);
            DataFim = DataInicio.AddMonths(1).AddDays(-1);
            Calcular();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
