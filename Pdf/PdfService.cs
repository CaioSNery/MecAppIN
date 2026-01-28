using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MecAppIN.Pdfs
{
    public static class PdfService
    {
        public static void AbrirPdf(string caminhoPdf)
        {
            if (!File.Exists(caminhoPdf))
            {
                MessageBox.Show(
                    "Arquivo PDF não encontrado.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = caminhoPdf,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao abrir o PDF:\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

       
        public static void ImprimirPdfSeguro(string caminhoPdf)
        {
            AbrirPdf(caminhoPdf);

            MessageBox.Show(
                "O PDF foi aberto.\nUtilize o botão IMPRIMIR",
                "Impressão",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
