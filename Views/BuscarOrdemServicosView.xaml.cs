using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MecAppIN.ViewModels;

namespace MecAppIN.Views
{
    public partial class BuscarOrdemServicosView : UserControl
    {
        public BuscarOrdemServicosView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 🔥 Se o clique veio da coluna "Pago", NÃO abre PDF
            if (e.OriginalSource is DependencyObject dep)
            {
                while (dep != null)
                {
                    if (dep is CheckBox || dep is DataGridCell cell && cell.Column.Header?.ToString() == "Pago")
                    {
                        e.Handled = true;
                        return;
                    }
                    dep = VisualTreeHelper.GetParent(dep);
                }
            }

            if (DataContext is not BuscarOrdemServicosViewModel vm)
                return;

            if (vm.OrdemSelecionada == null)
                return;

            var caminhoPdf = Path.Combine(
                @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                "PDFs",
                "OrdensDeServico",
                vm.OrdemSelecionada.Data.Year.ToString(),
                vm.OrdemSelecionada.Data.Month.ToString("D2"),
                $"OS_{vm.OrdemSelecionada.Id}.pdf"
            );

            if (!File.Exists(caminhoPdf))
            {
                MessageBox.Show(
                    "O PDF desta Ordem de Serviço não foi encontrado.",
                    "Arquivo não encontrado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = caminhoPdf,
                UseShellExecute = true
            });
        }
    }
}
