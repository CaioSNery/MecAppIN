using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
            col.Item().Text("SUA MARCA AQUI")
                .Bold()
                .FontSize(18);

            col.Item().Text("Rua ");
            col.Item().Text("Telefone: ");

            col.Item().PaddingTop(10).Row(row =>
{
    // ESQUERDA
    row.RelativeItem().Column(left =>
    {
        left.Item()
            .Text($"OS Nº: {_os.Id}  |  Motor: {_os.TipoMotor}");

        // 🔰 SELO DE OS PAGA
        if (_os.Pago)
        {
            left.Item()
                .PaddingTop(4)
                .Background(Colors.Green.Lighten3)
                .Padding(4)
                .AlignLeft()
                .Text("OS PAGA")
                .Bold()
                .FontSize(10)
                .FontColor(Colors.Green.Darken3);
        }
    });

    // DIREITA (DATAS)
    row.RelativeItem().AlignRight().Column(colData =>
    {
        colData.Item()
            .Text($"Data da OS: {_os.Data:dd/MM/yyyy}");

        if (_os.DataPagamento.HasValue)
        {
            colData.Item()
                .Text($"Pagamento em: {_os.DataPagamento:dd/MM/yyyy}")
                .FontSize(9)
                .Bold();
        }
    });
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
            var endereco = string.IsNullOrWhiteSpace(_os.ClienteEndereco)
                ? "Bahia"
                : _os.ClienteEndereco;
            var telefone = string.IsNullOrWhiteSpace(_os.ClienteTelefone)
            ? "-"
            : _os.ClienteTelefone;

            col.Item().Text($"Cliente: {_os.ClienteNome}      Telefone: {telefone}    Endereço: {endereco}");

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
        var servicos = _os.Itens
    .Where(i => i.Bloco == bloco && !i.IsPeca)
    .ToList();

        var pecas = _os.Itens
            .Where(i => i.Bloco == bloco && i.IsPeca)
            .ToList();

        BlocoMotorPdf(col.Item(), titulo, servicos, pecas);

    }




    void BlocoMotorPdf(
    IContainer container,
    string titulo,
    List<ItemOrdemServico> servicos,
    List<ItemOrdemServico> pecas)


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
    row.RelativeItem().PaddingRight(10).Element(c =>
    TabelaPadrao(c, "SERVIÇOS", servicos));

    row.RelativeItem().PaddingLeft(10).Element(c =>
        TabelaPadrao(c, "PEÇAS", pecas));


});

        });
    }

    void TabelaPadrao(
    IContainer container,
    string titulo,
    List<ItemOrdemServico> itens
)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(40);   // Qtde
                c.RelativeColumn();     // Descrição
                c.ConstantColumn(70);   // Valor
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

            // ===============================
            // ITENS (TODOS)
            // ===============================
            foreach (var item in itens)
            {
                table.Cell().Text(item.Quantidade.ToString());
                table.Cell().Text(item.Servico);
                string valorTexto;

                if (item.IsPeca)
                {
                    valorTexto = item.ValorUnitario > 0
                    ? item.ValorUnitario.ToString("C")
                    : ""; // peça do cliente, sem valor
                }
                else
                {
                    valorTexto = item.Total.ToString("C");
                }

                table.Cell().AlignRight().Text(valorTexto);

            }
        });
    }




    // ===============================
    // RODAPÉ / ASSINATURA
    // ===============================
    void Rodape(IContainer container)
    {
        var totalServicos = _os.Itens.Where(i => !i.IsPeca).Sum(i => i.Total);
        var totalPecas = _os.Itens.Where(i => i.IsPeca).Sum(i => i.Total);

        container.PaddingTop(25).Column(col =>
        {
            col.Item().LineHorizontal(1);

            // ===============================
            // ASSINATURA CENTRAL (LARGURA TOTAL)
            // ===============================
            col.Item()
                .PaddingTop(25)
                .AlignCenter()
                .Text("Assinatura do Cliente")
                .FontSize(16)
                .Bold();

            // ===============================
            // TOTAIS (ALINHADOS À DIREITA)
            // ===============================
            col.Item()
                .PaddingTop(10)
                .AlignRight()
                .Column(totais =>
                {
                    totais.Item()
                        .Text($"Total Serviços: {totalServicos:C}")
                        .FontSize(9)
                        .Bold();

                    totais.Item()
                        .Text($"Total Peças: {totalPecas:C}")
                        .FontSize(9)
                        .Bold();

                    totais.Item()
                        .Text($"TOTAL GERAL: {_os.Total:C}")
                        .FontSize(10)
                        .Bold();
                });
        });
    }

}
