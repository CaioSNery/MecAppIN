using MecAppIN.Models;
using MecAppIN.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MecAppIN.Views
{
    public partial class OrcamentosView : UserControl
    {
        public OrcamentosView()
        {
            InitializeComponent();
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            DependencyObject parent = sender as DependencyObject;

            while (parent != null && parent is not ScrollViewer)
                parent = VisualTreeHelper.GetParent(parent);

            if (parent is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(
                    scrollViewer.VerticalOffset - e.Delta
                );
            }
            }

        private void ClientesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem is Clientes cliente)
            {
                if (DataContext is OrcamentosViewModel vm)
                {
                    vm.ClienteSelecionado = cliente;
                    vm.TextoClienteDigitado = cliente.Nome;
                    vm.ClienteEndereco = cliente.Endereco;
                    vm.ClienteTelefone = cliente.Telefone;

                    lb.SelectedItem = null; // evita re-seleção
                }
            }
        }
    }
}
