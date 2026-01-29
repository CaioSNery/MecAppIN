using System.IO;
using MecAppIN.Models;

namespace MecAppIN.Helpers
{
    public static class PdfPathHelper
    {
        private static string BasePath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MecAppIN",
                "PDFs"
            );

        // ===============================
        // ORÇAMENTO
        // ===============================
        public static string ObterCaminhoOrcamento(Orcamentos o)
        {
            var pasta = Path.Combine(
                BasePath,
                "Orcamentos",
                o.Data.Year.ToString(),
                o.Data.ToString("MM-yyyy")
            );

            Directory.CreateDirectory(pasta);

            return Path.Combine(pasta, $"ORCAMENTO_{o.Id}.pdf");
        }

        // ===============================
        // ORDEM DE SERVIÇO
        // ===============================
        public static string ObterCaminhoOs(OrdemServicos os)
        {
            var pasta = Path.Combine(
                BasePath,
                "OrdensDeServico",
                os.Data.Year.ToString(),
                os.Data.ToString("MM-yyyy")
            );

            Directory.CreateDirectory(pasta);

            return Path.Combine(pasta, $"OS_{os.Id}.pdf");
        }

               
        // ===============================
        // FINANCEIRO
        // ===============================
        public static string ObterCaminhoFinanceiro(DateTime data)
        {
            var pasta = Path.Combine(
                BasePath,
                "Financeiro",
                data.Year.ToString(),
                data.ToString("MM-yyyy")
            );

            Directory.CreateDirectory(pasta);

            return Path.Combine(pasta, $"{data:yyyy-MM-dd}.pdf");
        }
    }
}

