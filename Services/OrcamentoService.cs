
using System.IO;
using MecAppIN.Data;
using MecAppIN.Models;
using QuestPDF.Fluent;

namespace MecAppIN.Services
{
    
        public class OrcamentoService
{
    public void ConverterEmOsEExcluir(Orcamentos orcamento)
    {
        using var db = new AppDbContext();

        // CRIA OS
        var os = new OrdemServicos
        {
            Data = DateTime.Now,
            ClienteId = orcamento.ClienteId,
            ClienteNome = orcamento.ClienteNome,
            Veiculo = orcamento.Veiculo,
            Placa = orcamento.Placa,
            Total = orcamento.Total,
            Itens = orcamento.Itens.Select(i => new ItemOrdemServico
            {
                Bloco = i.Bloco,
                Servico = i.Servico,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                IsPeca = i.IsPeca
            }).ToList()
        };

        db.OrdemServicos.Add(os);
        db.SaveChanges();

        // PDF OS
        var pdfOs = new OrdemServicoPdf(os);
        var caminhoPdfOs = Path.Combine(
            @"C:\Users\USER\Desktop\Projetos\MecAppIN",
            "PDFs",
            "OrdensDeServico",
            os.Data.Year.ToString(),
            os.Data.Month.ToString("D2"),
            $"OS_{os.Id}.pdf"
        );
        Directory.CreateDirectory(Path.GetDirectoryName(caminhoPdfOs));
        pdfOs.GeneratePdf(caminhoPdfOs);

        // PDF ORÇAMENTO
        var caminhoPdfOrcamento = Path.Combine(
            @"C:\Users\USER\Desktop\Projetos\MecAppIN",
            "PDFs",
            "Orcamentos",
            orcamento.Data.Year.ToString(),
            orcamento.Data.Month.ToString("D2"),
            $"ORCAMENTO_{orcamento.Id}.pdf"
        );
        if (File.Exists(caminhoPdfOrcamento))
            File.Delete(caminhoPdfOrcamento);

        // REMOVE ORÇAMENTO
        db.Orcamentos.Remove(orcamento);
        db.SaveChanges();
    }
}

    }
