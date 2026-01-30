using QuestPDF.Infrastructure;
using System.Windows;

namespace MecAppIN
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Licença do QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            // Captura de erro (diagnóstico)
            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show(ex.Exception.ToString(), "ERRO CAPTURADO");
                ex.Handled = true;
            };

            base.OnStartup(e);
        }
    }
}
