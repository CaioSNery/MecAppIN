
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MecAppIN.Views
{
    public partial class BuscarOrdemServicosView : UserControl
    {
        public BuscarOrdemServicosView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is not MecAppIN.ViewModels.BuscarOrdemServicosViewModel vm)
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