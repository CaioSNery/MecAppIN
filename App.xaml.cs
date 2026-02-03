using System.Windows;
using MecAppIN.Data;
using QuestPDF.Infrastructure;

namespace MecAppIN
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //  Licença
            QuestPDF.Settings.License = LicenseType.Community;

            //  Tratamento global de erro
            DispatcherUnhandledException += (s, ex) =>
            {
               MessageBox.Show(ex.Exception.Message, "Erro crítico");
               Environment.Exit(1);
            };


            // Inicialização do banco
            DbBootstrapper.InicializarBanco();


            base.OnStartup(e);
        }
    }
}
