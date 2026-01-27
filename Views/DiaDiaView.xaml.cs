
using System.Windows.Controls;
using MecAppIN.Data;
using MecAppIN.Models;
using System.Windows;
using System.Windows.Input;
using MecAppIN.ViewModels;

namespace MecAppIN.Views
{
    public partial class DiaDiaView : UserControl
    {
        public DiaDiaView()
        {
            InitializeComponent();

        }

        private void AbrirConsulta_Click(object sender, RoutedEventArgs e)
        {
            var janela = new ConsultaFinanceiroWindow
            {
                Owner = Application.Current.MainWindow
            };

            janela.ShowDialog();
        }



        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
                return;

            if (sender is not DataGrid grid)
                return;

            if (grid.SelectedItem is not LancamentoFinanceiro lancamento)
                return;

            var resposta = MessageBox.Show(
                "Tem certeza que deseja excluir este lançamento?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (resposta != MessageBoxResult.Yes)
            {
                e.Handled = true;
                return;
            }

            using var db = new AppDbContext();
            var registro = db.Lancamentos.Find(lancamento.Id);
            if (registro != null)
            {
                db.Lancamentos.Remove(registro);
                db.SaveChanges();
            }

            // Remove da tela
            if (DataContext is DiaDiaViewModel vm)
            {
                vm.Lancamentos.Remove(lancamento);
                vm.AtualizarTotais();

            }

            e.Handled = true;
        }

    }
}