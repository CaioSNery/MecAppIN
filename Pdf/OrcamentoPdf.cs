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
                page.Content().ScaleToFit().Element(Conteudo);
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
                    row.RelativeItem()
                        .Text($"ORÇAMENTO    Motor: {_orcamento.TipoMotor}")
                        .Bold();

                    row.RelativeItem().AlignRight()
                        .Text($"Data: {_orcamento.Data:dd/MM/yyyy}");
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
                var endereco = string.IsNullOrWhiteSpace(_orcamento.ClienteEndereco)
                ? "Bahia"
                : _orcamento.ClienteEndereco;
                var telefone = string.IsNullOrWhiteSpace(_orcamento.ClienteTelefone)
                ? "-"
                : _orcamento.ClienteTelefone;

                col.Item().Text($"Cliente: {_orcamento.ClienteNome}      Telefone: {telefone}    Endereço: {endereco}");

                col.Item().Text($"Veículo: {_orcamento.Veiculo}    Placa: {_orcamento.Placa}");

                ImprimirBloco(col, EBlocoMotor.Biela, "BIELA");
                ImprimirBloco(col, EBlocoMotor.Bloco, "BLOCO");
                ImprimirBloco(col, EBlocoMotor.Cabecote, "CABEÇOTE");
                ImprimirBloco(col, EBlocoMotor.Eixo, "EIXO");
                ImprimirBloco(col, EBlocoMotor.Motor, "MOTOR");
            });
        }

        void ImprimirBloco(ColumnDescriptor col, EBlocoMotor bloco, string titulo)
        {
            var servicos = _orcamento.Itens
                .Where(i => i.Bloco == bloco && !i.IsPeca)
                .ToList();

            var pecas = _orcamento.Itens
                .Where(i => i.Bloco == bloco && i.IsPeca)
                .ToList();

            if (!servicos.Any() && !pecas.Any())
                return;

            BlocoMotorPdf(col.Item(), titulo, servicos, pecas);
        }

        // ===============================
        // BLOCO PADRÃO (IGUAL OS)
        // ===============================
        void BlocoMotorPdf(
            IContainer container,
            string titulo,
            List<ItemOrcamento> servicos,
            List<ItemOrcamento> pecas)
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
                    row.RelativeItem().PaddingRight(10)
                        .Element(c => TabelaPadrao(c, "SERVIÇOS", servicos));

                    row.RelativeItem().PaddingLeft(10)
                        .Element(c => TabelaPadrao(c, "PEÇAS", pecas));
                });
            });
        }

        // ===============================
        // TABELA PADRÃO
        // ===============================
        void TabelaPadrao(
            IContainer container,
            string titulo,
            List<ItemOrcamento> itens)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(40);   // Qtde
                    c.RelativeColumn();     // Descrição
                    c.ConstantColumn(70);   // Valor TOTAL
                });

                table.Header(h =>
                {
                    h.Cell().ColumnSpan(3)
                        .BorderBottom(1)
                        .Text(titulo)
                        .Bold()
                        .AlignCenter();

                    h.Cell().Text("Qtde").Bold();
                    h.Cell().Text("Descrição").Bold();
                    h.Cell().AlignRight().Text("Total").Bold();
                });

                foreach (var item in itens)
                {
                    table.Cell().Text(item.Quantidade.ToString());
                    table.Cell().Text(item.Servico);

                    string valorTexto;

                    if (item.IsPeca)
                    {
                        valorTexto = item.ValorUnitario > 0
                            ? item.Total.ToString("C")
                            : "";
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
        // RODAPÉ
        // ===============================
        void Rodape(IContainer container)
        {
            var totalServicos = _orcamento.Itens
                .Where(i => !i.IsPeca)
                .Sum(i => i.ValorUnitario * i.Quantidade);

            var totalPecas = _orcamento.Itens
                .Where(i => i.IsPeca)
                .Sum(i => i.ValorUnitario * i.Quantidade);

            container.PaddingTop(25).Column(col =>
            {
                col.Item().LineHorizontal(1);

                col.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem()
                        .Text("Assinatura do Cliente")
                        .FontSize(9);

                    row.RelativeItem().AlignRight().Column(totais =>
                    {
                        totais.Item().Text($"Total Serviços: {totalServicos:C}")
                            .FontSize(9)
                            .Bold();

                        totais.Item().Text($"Total Peças: {totalPecas:C}")
                            .FontSize(9)
                            .Bold();

                        totais.Item().Text($"TOTAL GERAL: {_orcamento.Total:C}")
                            .FontSize(10)
                            .Bold();
                    });
                });
            });
        }
    }
}
