
using System.Windows;
using MecAppIN.Data;
using QuestPDF.Infrastructure;

namespace MecAppIN;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        QuestPDF.Settings.License = LicenseType.Community;
           // INICIALIZA BANCO
            DbBootstrapper.InicializarBanco(1059);

    }
}

