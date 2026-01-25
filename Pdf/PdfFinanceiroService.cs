using System.IO;
using MecAppIN.Models;
using QuestPDF.Fluent;


namespace MecAppIN.Pdf
{
    public static class PdfFinanceiroService
    {
        public static void GerarPdfDiario(DateTime data, List<LancamentoFinanceiro> lancamentos)
        {
            var basePath = Path.Combine(
                @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                "PDFs",
                "Financeiro",
                data.Year.ToString(),
                data.ToString("MM-yyyy")
            );

            Directory.CreateDirectory(basePath);

            var arquivo = Path.Combine(basePath, $"{data:yyyy-MM-dd}.pdf");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text($"Financeiro - {data:dd/MM/yyyy}")
                        .FontSize(18)
                        .Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Hora").Bold();
                            h.Cell().Text("Tipo").Bold();
                            h.Cell().Text("Forma").Bold();
                            h.Cell().Text("Valor").Bold();
                        });

                        foreach (var l in lancamentos)
                        {
                            table.Cell().Text(l.Data.ToString("HH:mm"));
                            table.Cell().Text(l.Tipo.ToString());
                            table.Cell().Text(l.Forma.ToString());
                            table.Cell().Text(l.Valor.ToString("C"));
                        }
                    });
                });
            }).GeneratePdf(arquivo);
        }
    }
}
