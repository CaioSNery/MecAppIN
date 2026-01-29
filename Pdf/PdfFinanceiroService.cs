
using MecAppIN.Enums;
using MecAppIN.Helpers;
using MecAppIN.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace MecAppIN.Pdf
{
    public static class PdfFinanceiroService
    {
        public static void GerarPdfDiario(DateTime data, List<LancamentoFinanceiro> lancamentos)
        {
            var arquivo = PdfPathHelper.ObterCaminhoFinanceiro(data);

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

                    page.Header()
                        .Text($"Financeiro - {data:dd/MM/yyyy}")
                        .FontSize(18)
                        .Bold()
                        .AlignCenter();


                    page.Content().PaddingTop(20).Row(row =>
{
    // =====================
    // ENTRADAS (ESQUERDA)
    // =====================
        row.RelativeItem().Column(col =>
        {
            col.Item()
                .Text("ENTRADAS")
                .FontSize(14)
                .Bold()
                .FontColor(Colors.Green.Darken2);

            col.Item().PaddingTop(5).Table(t =>
            {
                t.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(50);   // Hora
                    c.RelativeColumn();     // Descrição
                    c.ConstantColumn(70);   // Forma
                    c.ConstantColumn(80);   // Valor
                });

                t.Header(h =>
                {
                    h.Cell().Text("Hora").Bold();
                    h.Cell().Text("Descrição").Bold();
                    h.Cell().Text("Forma").Bold();
                    h.Cell().AlignRight().Text("Valor").Bold();
                });

                foreach (var e in entradas)
                {
                    t.Cell().Text(e.Data.ToString("HH:mm"));
                    t.Cell().Text(e.Descricao);
                    t.Cell().Text(e.Forma.ToString());
                    t.Cell().AlignRight().Text(e.Valor.ToString("C"));
                }

                t.Cell().ColumnSpan(3)
                    .AlignRight()
                    .Text("TOTAL ENTRADAS")
                    .Bold();

                t.Cell().AlignRight()
                    .Text(totalEntradas.ToString("C"))
                    .Bold();
            });
        });

        row.ConstantItem(20); // espaço central

    // =====================
    // SAÍDAS (DIREITA)
    // =====================
        row.RelativeItem().Column(col =>
        {
            col.Item()
                .Text("SAÍDAS")
                .FontSize(14)
                .Bold()
                .FontColor(Colors.Red.Darken2);

            col.Item().PaddingTop(5).Table(t =>
            {
                t.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(50);   // Hora
                    c.RelativeColumn();     // Descrição
                    c.ConstantColumn(70);   // Forma
                    c.ConstantColumn(80);   // Valor
                });

                t.Header(h =>
                {
                    h.Cell().Text("Hora").Bold();
                    h.Cell().Text("Descrição").Bold();
                    h.Cell().Text("Forma").Bold();
                    h.Cell().AlignRight().Text("Valor").Bold();
                });

                foreach (var s in saidas)
                {
                    t.Cell().Text(s.Data.ToString("HH:mm"));
                    t.Cell().Text(s.Descricao);
                    t.Cell().Text(s.Forma.ToString());
                    t.Cell().AlignRight().Text(s.Valor.ToString("C"));
                }

                t.Cell().ColumnSpan(3)
                    .AlignRight()
                    .Text("TOTAL SAÍDAS")
                    .Bold();

                t.Cell().AlignRight()
                    .Text(totalSaidas.ToString("C"))
                    .Bold();
            });
        });
    });



                    // =====================
                    // RODAPÉ
                    // =====================
                    page.Footer()
     .PaddingTop(20)
     .AlignRight()
     .Text($"RESULTADO DO DIA: {totalFinal:C}")
     .FontSize(16)
     .Bold();


                });
            }).GeneratePdf(arquivo);
        }
    }
}
