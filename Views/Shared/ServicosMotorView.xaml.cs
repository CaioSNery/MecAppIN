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
            if (e.Key != Key.Down && e.Key != Key.Enter)
                return;

            var grid = sender as DataGrid;
            if (grid == null)
                return;

            // Se estiver editando
            if (grid.CurrentCell != null)
            {
                // Finaliza edição da célula atual
                grid.CommitEdit(DataGridEditingUnit.Cell, true);
                grid.CommitEdit(DataGridEditingUnit.Row, true);

                int rowIndex = grid.Items.IndexOf(grid.CurrentItem);

                // Vai para a próxima linha, se existir
                if (rowIndex < grid.Items.Count - 1)
                {
                    grid.SelectedIndex = rowIndex + 1;
                    grid.CurrentCell = new DataGridCellInfo(
                        grid.Items[rowIndex + 1],
                        grid.Columns[0] // volta sempre para a coluna Quantidade
                    );

                    grid.BeginEdit();
                    e.Handled = true;
                }
            }
        }

    }
}
