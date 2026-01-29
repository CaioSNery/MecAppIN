using System.IO;
using MecAppIN.Models;

namespace MecAppIN.Helpers
{
    public static class PdfPathHelper
    {
        private static readonly string BasePath =
            @"C:\Users\USER\Desktop\Projetos\MecAppIN\PDFs";

        // ===============================
        // ORÇAMENTO
        // ===============================
        public static string ObterCaminhoOrcamento(Orcamentos o)
        {
            return Path.Combine(
                BasePath,
                "Orcamentos",
                o.Data.Year.ToString(),
                o.Data.Month.ToString("D2"),
                $"ORCAMENTO_{o.Id}.pdf"
            );
        }

        // ===============================
        // ORDEM DE SERVIÇO
        // ===============================
        public static string ObterCaminhoOs(OrdemServicos os)
        {
            return Path.Combine(
                BasePath,
                "OrdensDeServico",
                os.Data.Year.ToString(),
                os.Data.Month.ToString("D2"),
                $"OS_{os.Id}.pdf"
            );
        }
    }
}
