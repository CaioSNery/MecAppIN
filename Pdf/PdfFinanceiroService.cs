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

            var entradas = lancamentos
                .Where(l => l.Tipo == ETipoPagamento.Entrada)
                .ToList();

            var saidas = lancamentos
                .Where(l => l.Tipo == ETipoPagamento.Saida)
                .ToList();

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
                        col.Item().Text("ENTRADAS")
                            .FontSize(14)
                            .Bold()
                            .FontColor(Colors.Green.Darken2);

                        col.Item().Table(table =>
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
                                h.Cell().Text("Forma").Bold();
                                h.Cell().Text("Descrição").Bold();
                                h.Cell().Text("Valor").Bold();
                            });

                            foreach (var e in entradas)
                            {
                                table.Cell().Text(e.Data.ToString("HH:mm"));
                                table.Cell().Text(e.Forma.ToString());
                                table.Cell().Text("Entrada");
                                table.Cell().Text(e.Valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR")));
                            }

                            // TOTAL ENTRADAS
                            table.Cell().ColumnSpan(3)
                                 .Text("TOTAL ENTRADAS")
                                 .Bold();

                            table.Cell()
                                 .Text(totalEntradas.ToString("C", CultureInfo.GetCultureInfo("pt-BR")))
                                 .Bold();
                        });

                        col.Item().PaddingVertical(10);

                        // =====================
                        // SAÍDAS
                        // =====================
                        col.Item().Text("SAÍDAS")
                            .FontSize(14)
                            .Bold()
                            .FontColor(Colors.Red.Darken2);

                        col.Item().Table(table =>
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
                                h.Cell().Text("Forma").Bold();
                                h.Cell().Text("Descrição").Bold();
                                h.Cell().Text("Valor").Bold();
                            });

                            foreach (var s in saidas)
                            {
                                table.Cell().Text(s.Data.ToString("HH:mm"));
                                table.Cell().Text(s.Forma.ToString());
                                table.Cell().Text("Saída");
                                table.Cell().Text(s.Valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR")));
                            }

                            // TOTAL SAÍDAS
                            table.Cell().ColumnSpan(3)
                                 .Text("TOTAL SAÍDAS")
                                 .Bold();

                            table.Cell()
                                 .Text(totalSaidas.ToString("C", CultureInfo.GetCultureInfo("pt-BR")))
                                 .Bold();
                        });
                    });

                    // =====================
                    // RODAPÉ
                    // =====================
                    page.Footer()
                        .AlignRight()
                        .Text($"TOTAL DO DIA: {totalFinal.ToString("C", CultureInfo.GetCultureInfo("pt-BR"))}")
                        .FontSize(14)
                        .Bold();
                });
            }).GeneratePdf(arquivo);
        }
    }
}
