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
            if (DataContext is not BuscarOrdemServicosViewModel vm)
                return;

            if (vm.ReimprimirCommand.CanExecute(null))
                vm.ReimprimirCommand.Execute(null);
        }

    }
}
