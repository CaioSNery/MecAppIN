using MecAppIN.Models;
using MecAppIN.ViewModels;
using System.Windows.Controls;

namespace MecAppIN.Views
{
    public partial class OrcamentosView : UserControl
    {
        public OrcamentosView()
        {
            InitializeComponent();
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
