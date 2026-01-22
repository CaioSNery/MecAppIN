
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MecAppIN.Enums;

namespace MecAppIN.Views
{
    public partial class OrdemServicosView : UserControl
    {
        public OrdemServicosView()
        {
            InitializeComponent();
        }



        private void PecasBiela_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Quantidade = 1,
                IsPeca = true,
                Bloco = EBlocoMotor.Biela
            };
        }

        private void PecasBloco_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Quantidade = 1,
                IsPeca = true,
                Bloco = EBlocoMotor.Bloco
            };
        }

        private void PecasCabeCote_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Quantidade = 1,
                IsPeca = true,
                Bloco = EBlocoMotor.Cabecote
            };
        }

        private void PecasEixo_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Quantidade = 1,
                IsPeca = true,
                Bloco = EBlocoMotor.Eixo
            };
        }

        private void PecasMotor_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Quantidade = 1,
                IsPeca = true,
                Bloco = EBlocoMotor.Motor
            };
        }

        private void ServicosBiela_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                Quantidade = 1,
                ValorUnitario = 0,
                IsPeca = false,
                ValorEditavel = true
            };
        }
        private void ServicosBloco_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Quantidade = 1,
                ValorUnitario = 0,
                IsPeca = false,
                ValorEditavel = true
            };
        }
        private void ServicosCabecote_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Quantidade = 1,
                ValorUnitario = 0,
                IsPeca = false,
                ValorEditavel = true
            };
        }
        private void ServicosEixo_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                Quantidade = 1,
                ValorUnitario = 0,
                IsPeca = false,
                ValorEditavel = true
            };
        }
        private void ServicosMotor_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                Quantidade = 1,
                ValorUnitario = 0,
                IsPeca = false,
                ValorEditavel = true
            };
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

    }
}