using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MecAppIN.Models;
using MecAppIN.Enums;


namespace MecAppIN.Pdf
{
    public class OrcamentoPdf : IDocument
{
    private readonly Orcamentos _orcamento;

    public OrcamentoPdf(Orcamentos orcamento)
    {
        _orcamento = orcamento;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(15);

            page.Header().Element(Cabecalho);
            page.Content().Element(Conteudo);
            page.Footer().Element(Rodape);
        });
    }

    void Cabecalho(IContainer c)
    {
        c.Column(col =>
        {
            col.Item().Text("BNW RETÍFICA")
                .Bold()
                .FontSize(18);

            col.Item().Text("Rua Lauro de Freitas 104 - São Cristóvão - Salvador/BA");
            col.Item().Text("Telefone: (71)3252-6963/98722-0776");
            col.Item().PaddingTop(10).Row(r =>
            {
                r.RelativeItem().Text("OS Nº: 0");
                r.RelativeItem().AlignRight()
                    .Text($"Data: {_orcamento.Data:dd/MM/yyyy}");
            });

            col.Item().LineHorizontal(1);
        });
    }

    void Conteudo(IContainer c)
    {
        c.PaddingTop(10).Column(col =>
        {
            col.Item().Text($"Cliente: {_orcamento.ClienteNome}");
            col.Item().Text($"Veículo: {_orcamento.Veiculo}    Placa: {_orcamento.Placa}");

            foreach (EBlocoMotor bloco in Enum.GetValues(typeof(EBlocoMotor)))
            {
                var itens = _orcamento.Itens
                    .Where(i => i.Bloco == bloco)
                    .ToList();

                if (itens.Any())
                    Bloco(col.Item(), bloco.ToString().ToUpper(), itens);
            }
        });
    }

    void Bloco(IContainer c, string titulo, List<ItemOrcamento> itens)
    {
        c.PaddingTop(10).Column(col =>
        {
            col.Item().Background(Colors.Grey.Lighten3)
                .Padding(5).Text(titulo).Bold();

            col.Item().Table(t =>
            {
                t.ColumnsDefinition(cd =>
                {
                    cd.ConstantColumn(40);
                    cd.RelativeColumn();
                    cd.ConstantColumn(70);
                });

                t.Header(h =>
                {
                    h.Cell().Text("Qtde").Bold();
                    h.Cell().Text("Descrição").Bold();
                    h.Cell().AlignRight().Text("Valor").Bold();
                });

                foreach (var i in itens)
                {
                    t.Cell().Text(i.Quantidade.ToString());
                    t.Cell().Text(i.Servico);
                    t.Cell().AlignRight().Text(i.ValorUnitario > 0 ? i.ValorUnitario.ToString("C") : "");
                }
            });
        });
    }

    void Rodape(IContainer c)
    {
        c.PaddingTop(20).AlignRight()
            .Text($"TOTAL: {_orcamento.Total:C}")
            .Bold();
    }
}
}
