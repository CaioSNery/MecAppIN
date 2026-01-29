using System.Windows.Controls;
using System.Windows.Input;
using MecAppIN.Enums;

namespace MecAppIN.Views.Shared
{
    public partial class ServicosMotorView : UserControl
    {
        public ServicosMotorView()
        {
            InitializeComponent();
        }

        private void ServicosBiela_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasBiela_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                IsPeca = true,
                Quantidade = 1
            };
        }


        private void ServicosBloco_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasBloco_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                IsPeca = true,
                Quantidade = 1
            };
        }


        private void ServicosCabecote_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasCabeCote_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                IsPeca = true,
                Quantidade = 1
            };
        }


        private void ServicosEixo_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasEixo_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                IsPeca = true,
                Quantidade = 1
            };
        }


        private void ServicosMotor_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasMotor_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                IsPeca = true,
                Quantidade = 1
            };
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Enter)
                return;

            if (sender is not DataGrid grid)
                return;

            if (grid.CurrentItem == null || grid.CurrentCell.Column == null)
                return;

            // Finaliza edição atual
            grid.CommitEdit(DataGridEditingUnit.Cell, true);
            grid.CommitEdit(DataGridEditingUnit.Row, true);

            int rowIndex = grid.Items.IndexOf(grid.CurrentItem);

            if (e.Key == Key.Down || e.Key == Key.Enter)
            {
                if (rowIndex < grid.Items.Count - 1)
                {
                    MoverParaLinha(grid, rowIndex + 1);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (rowIndex > 0)
                {
                    MoverParaLinha(grid, rowIndex - 1);
                    e.Handled = true;
                }
            }
        }

        private void MoverParaLinha(DataGrid grid, int index)
        {
            grid.SelectedIndex = index;
            grid.CurrentCell = new DataGridCellInfo(
                grid.Items[index],
                grid.Columns[0] // coluna Quantidade
            );

            grid.BeginEdit();
        }


    }
}
