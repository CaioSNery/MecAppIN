using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Enums;
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

        public DateTime DataOrcamento { get; set; } = DateTime.Now;

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
        // ITENS
        // ===============================
        public ObservableCollection<ItemOrdemServico> ItensBiela { get; } = new();
        public ObservableCollection<ItemOrdemServico> ItensBloco { get; } = new();
        public ObservableCollection<ItemOrdemServico> ItensCabecote { get; } = new();
        public ObservableCollection<ItemOrdemServico> ItensEixo { get; } = new();
        public ObservableCollection<ItemOrdemServico> ItensMotor { get; } = new();

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
        // CONSTRUTOR NOVO
        // ===============================
        public OrcamentosViewModel()
        {
            Clientes = new ObservableCollection<Clientes>();

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

            _isCarregandoEdicao = true;

            TipoMotorSelecionado = orcamento.TipoMotor;
            Veiculo = orcamento.Veiculo;
            Placa = orcamento.Placa;

            if (orcamento.ClienteId.HasValue)
            {
                var cliente = db.Clientes.FirstOrDefault(c => c.Id == orcamento.ClienteId);
                if (cliente != null)
                    PreencherClienteEmModoEdicao(cliente);
            }
            else
            {
                TextoClienteDigitado = orcamento.ClienteNome;
                ClienteEndereco = orcamento.ClienteEndereco;
                ClienteTelefone = orcamento.ClienteTelefone;
            }

            LimparColecoes();

            foreach (var item in orcamento.Itens)
            {
                var novo = new ItemOrdemServico
                {
                    Servico = item.Servico,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario,
                    Bloco = item.Bloco,
                    IsPeca = item.IsPeca,
                    ValorEditavel = true
                };

                if (novo.IsPeca)
                    AdicionarPeca(novo);
                else
                    AdicionarServico(novo);
            }

            _isCarregandoEdicao = false;
        }

        // ===============================
        // SALVAR NOVO
        // ===============================
        private void SalvarOrcamento()
        {
            using var db = new AppDbContext();

            var orcamento = new Orcamentos
            {
                Data = DateTime.Now
            };

            PreencherEntidade(orcamento);
            db.Orcamentos.Add(orcamento);
            db.SaveChanges();

            _orcamentoId = orcamento.Id;

            GerarPdfRecarregado();
            MessageBox.Show("Orçamento salvo com sucesso!");
        }

        // ===============================
        // ATUALIZAR
        // ===============================
        private void AtualizarOrcamento()
        {
            using var db = new AppDbContext();

            var orcamento = db.Orcamentos
                .Include(o => o.Itens)
                .First(o => o.Id == _orcamentoId);

            PreencherEntidade(orcamento);
            db.SaveChanges();

            GerarPdfRecarregado();
            MessageBox.Show("Orçamento atualizado com sucesso!");
        }

        // ===============================
        // PREENCHIMENTO PADRÃO
        // ===============================
        private void PreencherEntidade(Orcamentos orcamento)
        {
            var itens = TodasColecoes()
                .SelectMany(c => c)
                .Where(i => i.Quantidade > 0)
                .ToList();

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
            {
                orcamento.Itens.Add(new ItemOrcamento
                {
                    Servico = i.Servico,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    Bloco = i.Bloco,
                    IsPeca = i.IsPeca
                });
            }
        }

        // ===============================
        // PDF (SEMPRE DO BANCO)
        // ===============================
        private void GerarPdfRecarregado()
        {
            using var db = new AppDbContext();

            var atualizado = db.Orcamentos
                .Include(o => o.Itens)
                .First(o => o.Id == _orcamentoId);

            var caminho = Path.Combine(
                @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                "PDFs",
                "Orcamentos",
                $"ORCAMENTO_{atualizado.Id}.pdf"
            );

            if (File.Exists(caminho))
                File.Delete(caminho); // 🔥 ISSO EVITA PDF FANTASMA

            new OrcamentoPdf(atualizado).GeneratePdf(caminho);
        }


        // ===============================
        // CONVERTER PARA OS
        // ===============================
        private void ConverterParaOs()
        {
            using var db = new AppDbContext();

            var orcamento = db.Orcamentos
                .Include(o => o.Itens)
                .First(o => o.Id == _orcamentoId);

            new OrcamentoService().ConverterEmOsEExcluir(orcamento);
            MessageBox.Show("Ordem de Serviço criada!");
        }

        // ===============================
        private bool PodeSalvar() =>
            !string.IsNullOrWhiteSpace(Veiculo) &&
            !string.IsNullOrWhiteSpace(Placa) &&
            TodasColecoes().SelectMany(c => c).Any();

        private void LimparColecoes()
        {
            foreach (var c in TodasColecoes())
                c.Clear();
        }

        private void AdicionarServico(ItemOrdemServico item)
        {
            switch (item.Bloco)
            {
                case EBlocoMotor.Biela: ItensBiela.Add(item); break;
                case EBlocoMotor.Bloco: ItensBloco.Add(item); break;
                case EBlocoMotor.Cabecote: ItensCabecote.Add(item); break;
                case EBlocoMotor.Eixo: ItensEixo.Add(item); break;
                case EBlocoMotor.Motor: ItensMotor.Add(item); break;
            }
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

        public new event PropertyChangedEventHandler PropertyChanged;
        protected new void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}

