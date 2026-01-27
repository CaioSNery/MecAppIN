using System.Globalization;
using System.IO;
using MecAppIN.Enums;
using MecAppIN.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

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

            var entradas = lancamentos.Where(l => l.Tipo == ETipoPagamento.Entrada).ToList();
            var saidas = lancamentos.Where(l => l.Tipo == ETipoPagamento.Saida).ToList();

            var totalEntradas = entradas.Sum(l => l.Valor);
            var totalSaidas = saidas.Sum(l => l.Valor);
            var totalFinal = totalEntradas - totalSaidas;


            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // =====================
                    // CABEÇALHO
                    // =====================
                    page.Header()
                        .Text($"Financeiro - {data:dd/MM/yyyy}")
                        .FontSize(18)
                        .Bold()
                        .AlignCenter();

                    page.Content().Column(col =>
                    {
                        // =====================
                        // ENTRADAS
                        // =====================
                        col.Item().Text("ENTRADAS").Bold().FontSize(14);

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            t.Header(h =>
                            {
                                h.Cell().Text("Hora").Bold();
                                h.Cell().Text("Forma").Bold();
                                h.Cell().Text("Descrição").Bold();
                                h.Cell().Text("Valor").Bold();
                            });

                            foreach (var e in entradas)
                            {
                                t.Cell().Text(e.Data.ToString("HH:mm"));
                                t.Cell().Text(e.Forma.ToString());
                                t.Cell().Text(e.Descricao);
                                t.Cell().Text(e.Valor.ToString("C"));
                            }

                            t.Cell().ColumnSpan(3).Text("TOTAL ENTRADAS").Bold();
                            t.Cell().Text(totalEntradas.ToString("C")).Bold();
                        });


                        col.Item().PaddingVertical(10);

                        // =====================
                        // SAÍDAS
                        // =====================
                        col.Item().PaddingTop(10);
                        col.Item().Text("SAÍDAS").Bold().FontSize(14);

                        col.Item().Table(t =>
{
    t.ColumnsDefinition(c =>
    {
        c.RelativeColumn();
        c.RelativeColumn();
        c.RelativeColumn();
        c.RelativeColumn();
    });

    t.Header(h =>
    {
        h.Cell().Text("Hora").Bold();
        h.Cell().Text("Forma").Bold();
        h.Cell().Text("Descrição").Bold();
        h.Cell().Text("Valor").Bold();
    });

    foreach (var e in entradas)
    {
        t.Cell().Text(e.Data.ToString("HH:mm"));
        t.Cell().Text(e.Forma.ToString());
        t.Cell().Text(e.Descricao);
        t.Cell().Text(e.Valor.ToString("C"));
    }

    t.Cell().ColumnSpan(3).Text("TOTAL SAIDAS").Bold();
    t.Cell().Text(totalEntradas.ToString("C")).Bold();

});
                    });

                    // =====================
                    // RODAPÉ
                    // =====================
                    page.Footer()
    .AlignRight()
    .Text($"TOTAL DO DIA: {totalFinal:C}")
    .FontSize(14)
    .Bold();

                });
            }).GeneratePdf(arquivo);
        }
    }
}
