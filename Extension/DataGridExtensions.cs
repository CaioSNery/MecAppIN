using System.Windows.Controls;

namespace MecAppIN.Extension
{
    public static class DataGridExtensions
    {
         public static bool IsEditing(this DataGrid grid)
    {
        return grid.CommitEdit(DataGridEditingUnit.Cell, false) == false;
    }
    }
}