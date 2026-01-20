using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MecAppIN.Models;
using MecAppIN.Enums;

public class OrdemServicoPdf : IDocument
{
    private readonly OrdemServicos _os;

    public OrdemServicoPdf(OrdemServicos os)
    {
        _os = os;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(15);

            page.Header().Element(Cabecalho);

            page.Content()
                .ScaleToFit()
                .Element(Conteudo);

            page.Footer().Element(Rodape);
        });
    }


    // ===============================
    // CABEÇALHO
    // ===============================
    void Cabecalho(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("BNW RETÍFICA")
                .Bold()
                .FontSize(18);

            col.Item().Text("Rua Lauro de Freitas 104 - São Cristóvão - Salvador/BA");
            col.Item().Text("Telefone: (71)3252-6963/98722-0776");

            col.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text($"OS Nº: {_os.Id}");
                row.RelativeItem().AlignRight()
                    .Text($"Data: {_os.Data:dd/MM/yyyy}");
            });

            col.Item().LineHorizontal(1);
        });
    }

    // ===============================
    // CONTEÚDO
    // ===============================
    void Conteudo(IContainer container)
    {
        container.PaddingTop(10).Column(col =>
        {
            col.Item().Text($"Cliente: {_os.ClienteNome}");
            col.Item().Text($"Veículo: {_os.Veiculo}    Placa: {_os.Placa}");

            ImprimirBloco(col, EBlocoMotor.Biela, "BIELA");
            ImprimirBloco(col, EBlocoMotor.Bloco, "BLOCO");
            ImprimirBloco(col, EBlocoMotor.Cabecote, "CABEÇOTE");
            ImprimirBloco(col, EBlocoMotor.Eixo, "EIXO");
            ImprimirBloco(col, EBlocoMotor.Motor, "MOTOR");
        });
    }

    void ImprimirBloco(ColumnDescriptor col, EBlocoMotor bloco, string titulo)
    {
        var itens = _os.Itens
            .Where(i => i.Bloco == bloco)
            .ToList();

        BlocoMotorPdf(col.Item(), titulo, itens);
    }



    void BlocoMotorPdf(IContainer container, string titulo, List<ItemOrdemServico> itens)
    {
        container.PaddingVertical(10).Column(col =>
        {
            col.Item()
   .Background(Colors.Grey.Lighten3)
   .Padding(5)
   .Text(titulo)
   .Bold()
   .FontSize(11);


            col.Item().Row(row =>
            {
                row.RelativeItem().Element(c =>
                    TabelaPadrao(c, "SERVIÇOS", itens));

                row.RelativeItem().Element(c =>
                    TabelaPadrao(c, "PEÇAS", new()));
            });
        });
    }

    void TabelaPadrao(IContainer container, string titulo, List<ItemOrdemServico> itens)
    {
        const int linhasFixas = 4; 

        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(40);   
                c.RelativeColumn();     
                c.ConstantColumn(60);   
            });

            // ===============================
            // CABEÇALHO
            // ===============================
            table.Header(h =>
            {
                h.Cell().ColumnSpan(3)
                    .BorderBottom(1)
                    .Text(titulo)
                    .Bold()
                    .AlignCenter();

                h.Cell().Text("Qtde").Bold();
                h.Cell().Text("Descrição").Bold();
                h.Cell().AlignRight().Text("Valor").Bold();
            });

            int linhasUsadas = 0;

            // ===============================
            // ITENS REAIS
            // ===============================
            foreach (var item in itens.Take(linhasFixas))
            {
                table.Cell().Text(item.Quantidade.ToString());
                table.Cell().Text(item.Servico);
                table.Cell().AlignRight().Text(item.Total.ToString("C"));

                linhasUsadas++;
            }

            // ===============================
            // LINHAS EM BRANCO (até completar 4)
            // ===============================
            for (int i = linhasUsadas; i < linhasFixas; i++)
            {
                table.Cell().Height(18);
                table.Cell();
                table.Cell();
            }
        });
    }



    // ===============================
    // RODAPÉ / ASSINATURA
    // ===============================
    void Rodape(IContainer container)
    {
        container.PaddingTop(30).Column(col =>
        {
            col.Item().LineHorizontal(1);
            col.Item().AlignCenter()
                .Text("Assinatura do Cliente");

            col.Item().PaddingTop(10)
                .AlignRight()
                .Text($"TOTAL GERAL: {_os.Total:C}")
                .Bold();
        });
    }



}
