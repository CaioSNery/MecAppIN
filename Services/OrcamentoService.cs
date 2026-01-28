
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

            Clientes cliente = null;

            if (orcamento.ClienteId.HasValue)
            {
                cliente = db.Clientes
                    .FirstOrDefault(c => c.Id == orcamento.ClienteId.Value);
            }

            var os = new OrdemServicos
            {
                Data = DateTime.Now,
                ClienteId = orcamento.ClienteId,
                ClienteNome = orcamento.ClienteNome,
                

                // COPIA REAL DOS DADOS
                ClienteEndereco = cliente?.Endereco ?? string.Empty,
                ClienteTelefone = cliente?.Telefone ?? string.Empty,

                //TIPO DO MOTOR
                TipoMotor = orcamento.TipoMotor,

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

            // Remove orçamento
            db.Orcamentos.Remove(orcamento);
            db.SaveChanges();
        }

    }

}
