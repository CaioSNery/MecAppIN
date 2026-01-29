using System;
using System.Globalization;
using System.Windows.Data;

namespace MecAppIN.Converters;
public class DataPagamentoTooltipConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime data)
            return $"OS paga em {data:dd/MM/yyyy HH:mm}";

        return "OS não paga";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
