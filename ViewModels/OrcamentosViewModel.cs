using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Enums;
using MecAppIN.Helpers;
using MecAppIN.Models;
using MecAppIN.Pdf;
using MecAppIN.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MecAppIN.ViewModels
{
    public class OrcamentosViewModel : ClienteBaseViewModel, INotifyPropertyChanged
    {
        // ===============================
        // CABEÇALHO
        // ===============================
        private string _veiculo;
        public string Veiculo
        {
            get => _veiculo;
            set { _veiculo = value; OnPropertyChanged(); }
        }

        private string _placa;
        public string Placa
        {
            get => _placa;
            set { _placa = value; OnPropertyChanged(); }
        }

        // ===============================
        // MOTOR
        // ===============================
        public ObservableCollection<string> TiposMotor { get; } =
            new() { "Gasolina", "Diesel" };

        private string _tipoMotorSelecionado = "Gasolina";
        public string TipoMotorSelecionado
        {
            get => _tipoMotorSelecionado;
            set { _tipoMotorSelecionado = value; OnPropertyChanged(); }
        }

        // ===============================
        // SERVIÇOS
        // ===============================
        public ObservableCollection<ItemOrdemServico> ItensBiela { get; } = new();
        public ObservableCollection<ItemOrdemServico> ItensBloco { get; } = new();
        public ObservableCollection<ItemOrdemServico> ItensCabecote { get; } = new();
        public ObservableCollection<ItemOrdemServico> ItensEixo { get; } = new();
        public ObservableCollection<ItemOrdemServico> ItensMotor { get; } = new();

        // ===============================
        // PEÇAS
        // ===============================
        public ObservableCollection<ItemOrdemServico> PecasBiela { get; } = new();
        public ObservableCollection<ItemOrdemServico> PecasBloco { get; } = new();
        public ObservableCollection<ItemOrdemServico> PecasCabecote { get; } = new();
        public ObservableCollection<ItemOrdemServico> PecasEixo { get; } = new();
        public ObservableCollection<ItemOrdemServico> PecasMotor { get; } = new();

        private ObservableCollection<ItemOrdemServico>[] TodasColecoes() =>
            new[]
            {
                ItensBiela, ItensBloco, ItensCabecote, ItensEixo, ItensMotor,
                PecasBiela, PecasBloco, PecasCabecote, PecasEixo, PecasMotor
            };

        // ===============================
        // TOTAIS
        // ===============================
        public decimal TotalGeral =>
            TodasColecoes().SelectMany(c => c).Sum(i => i.Total);

        // ===============================
        // CONTROLE
        // ===============================
        private int _orcamentoId;
        public bool Editando => _orcamentoId > 0;

        // ===============================
        // COMMANDS
        // ===============================
        public ICommand SalvarOrcamentoCommand { get; }
        public ICommand AtualizarOrcamentoCommand { get; }
        public ICommand ConverterParaOsCommand { get; }

        // ===============================
        // CONSTRUTOR 
        // ===============================
        public OrcamentosViewModel()
        {
            using var db = new AppDbContext();
            Clientes = new ObservableCollection<Clientes>(db.Clientes.ToList());

            CarregarServicosPadrao();

            SalvarOrcamentoCommand = new RelayCommand(SalvarOrcamento, PodeSalvar);
            AtualizarOrcamentoCommand = new RelayCommand(AtualizarOrcamento, () => Editando);
            ConverterParaOsCommand = new RelayCommand(ConverterParaOs, () => Editando);
        }

        // ===============================
        // CONSTRUTOR EDIÇÃO
        // ===============================
        public OrcamentosViewModel(int orcamentoId) : this()
        {
            using var db = new AppDbContext();

            var orcamento = db.Orcamentos
                .Include(o => o.Itens)
                .First(o => o.Id == orcamentoId);

            _orcamentoId = orcamento.Id;

            Veiculo = orcamento.Veiculo;
            Placa = orcamento.Placa;
            TipoMotorSelecionado = orcamento.TipoMotor;

            if (orcamento.ClienteId.HasValue)
            {
                ClienteSelecionado = db.Clientes
                    .First(c => c.Id == orcamento.ClienteId.Value);
            }
            else
            {
                TextoClienteDigitado = orcamento.ClienteNome;
                ClienteEndereco = orcamento.ClienteEndereco;
                ClienteTelefone = orcamento.ClienteTelefone;
            }

            AplicarItensDoBanco(orcamento);
        }

        // ===============================
        // SERVIÇOS PADRÃO
        // ===============================
        private void CarregarServicosPadrao()
        {
            CarregarItensBiela();
            CarregarItensBloco();
            CarregarItensCabecote();
            CarregarItensEixo();
            CarregarItensMotor();
        }

        private void CarregarItensBiela()
        {
            ItensBiela.Clear();
            foreach (var item in ItensMotorPadraoHelper.CriarItensBiela())
                ItensBiela.Add(item);
        }

        private void CarregarItensBloco()
        {
            ItensBloco.Clear();
            foreach (var item in ItensMotorPadraoHelper.CriarItensBloco())
                ItensBloco.Add(item);
        }

        public void CarregarItensCabecote()
        {
            ItensCabecote.Clear();
            foreach (var item in ItensMotorPadraoHelper.CriarItensCabecote())
                ItensCabecote.Add(item);
        }

        public void CarregarItensEixo()
        {
            ItensEixo.Clear();
            foreach (var item in ItensMotorPadraoHelper.CriarItensEixo())
                ItensEixo.Add(item);
        }

        public void CarregarItensMotor()
        {
            ItensMotor.Clear();
            foreach (var item in ItensMotorPadraoHelper.CriarItensMotor())
                ItensMotor.Add(item);
        }


        // ===============================
        // APLICAR ITENS
        // ===============================
        private void AplicarItensDoBanco(Orcamentos orcamento)
        {
            foreach (var item in orcamento.Itens)
            {
                if (item.IsPeca)
                {
                    AdicionarPeca(new ItemOrdemServico
                    {
                        Bloco = item.Bloco,
                        Servico = item.Servico,
                        Quantidade = item.Quantidade,
                        ValorUnitario = item.ValorUnitario,
                        IsPeca = true
                    });
                    continue;
                }

                var lista = ObterListaServicos(item.Bloco);
                var servico = lista.FirstOrDefault(s => s.Servico == item.Servico);
                if (servico != null)
                    servico.Quantidade = item.Quantidade;
            }
        }

        private ObservableCollection<ItemOrdemServico> ObterListaServicos(EBlocoMotor bloco)
        {
            return bloco switch
            {
                EBlocoMotor.Biela => ItensBiela,
                EBlocoMotor.Bloco => ItensBloco,
                EBlocoMotor.Cabecote => ItensCabecote,
                EBlocoMotor.Eixo => ItensEixo,
                EBlocoMotor.Motor => ItensMotor,
                _ => null
            };
        }

        private void AdicionarPeca(ItemOrdemServico item)
        {
            switch (item.Bloco)
            {
                case EBlocoMotor.Biela: PecasBiela.Add(item); break;
                case EBlocoMotor.Bloco: PecasBloco.Add(item); break;
                case EBlocoMotor.Cabecote: PecasCabecote.Add(item); break;
                case EBlocoMotor.Eixo: PecasEixo.Add(item); break;
                case EBlocoMotor.Motor: PecasMotor.Add(item); break;
            }
        }

        // ===============================
        // SALVAR / ATUALIZAR
        // ===============================
        private void SalvarOrcamento()
        {
            using var db = new AppDbContext();
            var orcamento = new Orcamentos { Data = System.DateTime.Now };
            PreencherEntidade(orcamento);
            db.Orcamentos.Add(orcamento);
            db.SaveChanges();
            _orcamentoId = orcamento.Id;
            GerarPdfRecarregado();
            MessageBox.Show("Orçamento salvo com sucesso!");
        }

        private void AtualizarOrcamento()
        {
            using var db = new AppDbContext();
            var orcamento = db.Orcamentos.Include(o => o.Itens).First(o => o.Id == _orcamentoId);
            PreencherEntidade(orcamento);
            db.SaveChanges();
            GerarPdfRecarregado();
            MessageBox.Show("Orçamento atualizado com sucesso!");
        }

        private void PreencherEntidade(Orcamentos orcamento)
        {
            var itens = TodasColecoes().SelectMany(c => c).Where(i => i.Quantidade > 0).ToList();
            orcamento.Itens.Clear();
            orcamento.ClienteId = ClienteSelecionado?.Id;
            orcamento.ClienteNome = ClienteSelecionado?.Nome ?? TextoClienteDigitado;
            orcamento.ClienteEndereco = ClienteSelecionado?.Endereco ?? ClienteEndereco;
            orcamento.ClienteTelefone = ClienteSelecionado?.Telefone ?? ClienteTelefone;
            orcamento.Veiculo = Veiculo;
            orcamento.Placa = Placa;
            orcamento.TipoMotor = TipoMotorSelecionado;
            orcamento.Total = itens.Sum(i => i.Total);

            foreach (var i in itens)
                orcamento.Itens.Add(new ItemOrcamento
                {
                    Servico = i.Servico,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    Bloco = i.Bloco,
                    IsPeca = i.IsPeca
                });
        }

        private void GerarPdfRecarregado()
        {
            using var db = new AppDbContext();

            var atualizado = db.Orcamentos
                .AsNoTracking()
                .Include(o => o.Itens)
                .First(o => o.Id == _orcamentoId);

            var caminho = PdfPathHelper.ObterCaminhoOrcamento(atualizado);

            Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);

            if (File.Exists(caminho))
                File.Delete(caminho);

            new OrcamentoPdf(atualizado).GeneratePdf(caminho);
        }

        private void ConverterParaOs()
{
    try
    {
        var service = new OrcamentoService();

        // 1️⃣ Converte e retorna o ID da OS
        var osId = service.ConverterEmOsEExcluir(_orcamentoId);

        // 2️⃣ Recarrega a OS do banco (CRÍTICO)
        using var db = new AppDbContext();

        var os = db.OrdemServicos
    .AsNoTracking()
    .Include(o => o.Itens)
    .First(o => o.Id == osId);

// 🔒 GARANTIA FINAL
if (os.Data == default)
    os.Data = DateTime.Now;

        // 3️⃣ Gera PDF DA OS
        GerarPdfOs(os);

        MessageBox.Show("Ordem de Serviço criada!");
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "Erro");
    }
}




        private void GerarPdfOs(OrdemServicos os)
{
    var caminho = PdfPathHelper.ObterCaminhoOs(os);

    Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);

    if (File.Exists(caminho))
        File.Delete(caminho);

    new OrdemServicoPdf(os).GeneratePdf(caminho);
}





        private bool PodeSalvar() =>
            !string.IsNullOrWhiteSpace(Veiculo) &&
            !string.IsNullOrWhiteSpace(Placa);

        public new event PropertyChangedEventHandler PropertyChanged;
        protected new void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
