using System.IO;
using MecAppIN.Data;
using MecAppIN.Helpers;
using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace MecAppIN.Services
{
    public class OrcamentoService
    {
        public void ConverterEmOsEExcluir(Orcamentos orcamento)
        {
            using var db = new AppDbContext();

            var orcamentoDb = db.Orcamentos
                .Include(o => o.Itens)
                .First(o => o.Id == orcamento.Id);

            // ===============================
            // CRIA OS
            // ===============================
            var os = new OrdemServicos
            {
                Data = DateTime.Now,
                ClienteId = orcamentoDb.ClienteId,
                ClienteNome = orcamentoDb.ClienteNome,
                ClienteEndereco = orcamentoDb.ClienteEndereco,
                ClienteTelefone = orcamentoDb.ClienteTelefone,
                Veiculo = orcamentoDb.Veiculo,
                Placa = orcamentoDb.Placa,
                TipoMotor = orcamentoDb.TipoMotor,
                Total = orcamentoDb.Total,
                Pago = false,
                Itens = orcamentoDb.Itens.Select(i => new ItemOrdemServico
                {
                    Servico = i.Servico,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    Bloco = i.Bloco,
                    IsPeca = i.IsPeca
                }).ToList()
            };

            db.OrdemServicos.Add(os);
            db.SaveChanges(); // 👈 precisa do ID da OS

            // ===============================
            // GERA PDF DA OS
            // ===============================
            var caminhoOs = PdfPathHelper.ObterCaminhoOs(os);
            new OrdemServicoPdf(os).GeneratePdf(caminhoOs);

            // ===============================
            // REMOVE PDF DO ORÇAMENTO
            // ===============================
            var caminhoOrcamento = PdfPathHelper.ObterCaminhoOrcamento(orcamentoDb);

            if (File.Exists(caminhoOrcamento))
                File.Delete(caminhoOrcamento);

            // ===============================
            // REMOVE ORÇAMENTO DO BANCO
            // ===============================
            db.Orcamentos.Remove(orcamentoDb);
            db.SaveChanges();
        }
    }
}
