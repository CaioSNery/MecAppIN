using QuestPDF.Infrastructure;
using System.Windows;

namespace MecAppIN
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 1️⃣ Licença do QuestPDF (SEMPRE PRIMEIRO)
            QuestPDF.Settings.License = LicenseType.Community;

            // 2️⃣ Captura de erro (diagnóstico)
            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show(ex.Exception.ToString(), "ERRO CAPTURADO");
                ex.Handled = true;
            };

            base.OnStartup(e);
        }
    }
}
