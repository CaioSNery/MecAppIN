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
            page.Margin(20);
            page.Size(PageSizes.A4);

            page.Header().Element(Cabecalho);
            page.Content().Element(Conteudo);
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

            col.Item().Text("Rua Lauro de Freitas - São Cristóvão - Salvador/BA");
            col.Item().Text("Telefone: (71) 9 9999-9999");

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

            BlocoMotorPdf(
                col.Item(),
                "BIELA",
                _os.Itens.Where(i => i.Bloco == EBlocoMotor.Biela).ToList()
            );

            BlocoMotorPdf(
                col.Item(),
                "BLOCO",
                _os.Itens.Where(i => i.Bloco == EBlocoMotor.Bloco).ToList()
            );

            BlocoMotorPdf(
                col.Item(),
                "CABEÇOTE",
                _os.Itens.Where(i => i.Bloco == EBlocoMotor.Cabecote).ToList()
            );

            BlocoMotorPdf(
                col.Item(),
                "EIXO",
                _os.Itens.Where(i => i.Bloco == EBlocoMotor.Eixo).ToList()
            );

            BlocoMotorPdf(
                col.Item(),
                "MOTOR",
                _os.Itens.Where(i => i.Bloco == EBlocoMotor.Motor).ToList()
            );
        });
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
   .FontSize(12);


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
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(40);
                c.RelativeColumn();
                c.ConstantColumn(60);
            });

            table.Header(h =>
            {
                h.Cell().ColumnSpan(3)
                    .Text(titulo)
                    .Bold()
                    .AlignCenter();

                h.Cell().Text("Qtde").Bold();
                h.Cell().Text("Descrição").Bold();
                h.Cell().Text("Valor").Bold();
            });

            if (!itens.Any())
            {
                // linhas em branco (igual OS de papel)
                for (int i = 0; i < 5; i++)
                {
                    table.Cell().Height(18);
                    table.Cell();
                    table.Cell();
                }
            }
            else
            {
                foreach (var item in itens)
                {
                    table.Cell().Text(item.Quantidade.ToString());
                    table.Cell().Text(item.Servico);
                    table.Cell().Text(item.Total.ToString("C"));
                }
            }
        });
    }




    // ===============================
    // TABELA SERVIÇOS
    // ===============================
    void TabelaServicos(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(40);   // Qtde
                columns.RelativeColumn();     // Descrição
                columns.ConstantColumn(60);   // Valor
            });

            table.Header(h =>
            {
                h.Cell().Text("Qtde").Bold();
                h.Cell().Text("Serviço").Bold();
                h.Cell().Text("Valor").Bold();
            });

            foreach (var item in _os.Itens)
            {
                table.Cell().Text(item.Quantidade.ToString());
                table.Cell().Text(item.Servico);
                table.Cell().Text(
                    (item.Quantidade * item.ValorUnitario).ToString("C")
                );
            }
        });
    }

    // ===============================
    // TABELA PEÇAS (FUTURO)
    // ===============================
    void TabelaPecas(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(40);
                columns.RelativeColumn();
                columns.ConstantColumn(60);
            });

            table.Header(h =>
            {
                h.Cell().Text("Qtde").Bold();
                h.Cell().Text("Peça").Bold();
                h.Cell().Text("Valor").Bold();
            });

            // vazio por enquanto
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
