using System.Windows.Controls;
using System.Windows.Input;
using MecAppIN.ViewModels;

namespace MecAppIN.Views
{
    public partial class BuscarOrcamentosView : UserControl
    {
        public BuscarOrcamentosView()
        {
            InitializeComponent();
        }

        
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is BuscarOrcamentosViewModel vm &&
                vm.AbrirPdfCommand.CanExecute(null))
            {
                vm.AbrirPdfCommand.Execute(null);
            }
        }

        
        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is not BuscarOrcamentosViewModel vm)
                return;

            
            if (e.Key == Key.Enter && vm.ImprimirCommand.CanExecute(null))
            {
                vm.ImprimirCommand.Execute(null);
                e.Handled = true;
                return;
            }

            
            if (e.Key == Key.P &&
                Keyboard.Modifiers == ModifierKeys.Control &&
                vm.ImprimirCommand.CanExecute(null))
            {
                vm.ImprimirCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}
