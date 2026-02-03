using MecAppIN.Data;
using MecAppIN.Helpers;
using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace MecAppIN.Services
{
    public class OrcamentoService
    {
        public int ConverterEmOsEExcluir(int orcamentoId)
        {
            using var db = new AppDbContext();
            using var tx = db.Database.BeginTransaction();

            try
            {
                // ===============================
                // CARREGA ORÇAMENTO
                // ===============================
                var orcamento = db.Orcamentos
                    .Include(o => o.Itens)
                    .FirstOrDefault(o => o.Id == orcamentoId);

                if (orcamento == null)
                    throw new Exception("Orçamento não encontrado.");

                // ===============================
                // EXCLUI PDF DO ORÇAMENTO
                // ===============================
                var caminhoPdfOrcamento =
                    PdfPathHelper.ObterCaminhoOrcamento(orcamento);

                if (File.Exists(caminhoPdfOrcamento))
                    File.Delete(caminhoPdfOrcamento);

                // ===============================
                // GERA NÚMERO DE OS
                // ===============================
                var sequencia = db.SequenciasOs
                    .First(s => s.TipoMotor == orcamento.TipoMotor);

                int novoNumeroOs;
                do
                {
                    sequencia.UltimoNumero++;
                    novoNumeroOs = sequencia.UltimoNumero;
                }
                while (db.OrdemServicos.Any(o => o.Id == novoNumeroOs));

                // ===============================
                // CRIA OS
                // ===============================
                var os = new OrdemServicos
                {
                    Id = novoNumeroOs,
                    Data = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local),
                    ClienteId = orcamento.ClienteId,
                    ClienteNome = orcamento.ClienteNome,
                    ClienteEndereco = orcamento.ClienteEndereco,
                    ClienteTelefone = orcamento.ClienteTelefone,
                    Veiculo = orcamento.Veiculo,
                    Placa = orcamento.Placa,
                    TipoMotor = orcamento.TipoMotor,
                    Total = orcamento.Total,
                    OrcamentoId = orcamento.Id,
                    Itens = orcamento.Itens.Select(i => new ItemOrdemServico
                    {
                        Servico = i.Servico,
                        Quantidade = i.Quantidade,
                        ValorUnitario = i.ValorUnitario,
                        Bloco = i.Bloco,
                        IsPeca = i.IsPeca
                    }).ToList()
                };

                db.OrdemServicos.Add(os);

                // ===============================
                // REMOVE ORÇAMENTO
                // ===============================
                db.Orcamentos.Remove(orcamento);

                db.SaveChanges();
                tx.Commit();

                return os.Id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
