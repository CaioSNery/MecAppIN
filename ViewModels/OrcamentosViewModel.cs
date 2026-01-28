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
            set
            {
                _veiculo = value;
                OnPropertyChanged();
                AtualizarEstadoSalvar();
            }
        }

        private string _placa;
        public string Placa
        {
            get => _placa;
            set
            {
                _placa = value;
                OnPropertyChanged();
                AtualizarEstadoSalvar();
            }
        }

        public DateTime DataOrcamento { get; set; } = DateTime.Now;

        // ===============================
        // ITENS (IGUAL ORDEM DE SERVIÇO)
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

        // ===============================
        // TOTAIS
        // ===============================
        public decimal TotalServicos =>
            ItensBiela.Sum(i => i.Total)
          + ItensBloco.Sum(i => i.Total)
          + ItensCabecote.Sum(i => i.Total)
          + ItensEixo.Sum(i => i.Total)
          + ItensMotor.Sum(i => i.Total);

        public decimal TotalPecas =>
            PecasBiela.Sum(i => i.Total)
          + PecasBloco.Sum(i => i.Total)
          + PecasCabecote.Sum(i => i.Total)
          + PecasEixo.Sum(i => i.Total)
          + PecasMotor.Sum(i => i.Total);

        public decimal TotalGeral => TotalServicos + TotalPecas;

        // ===============================
        // COMMANDS
        // ===============================
        public ICommand SalvarOrcamentoCommand { get; }
        public ICommand ImprimirOrcamentoCommand { get; }
        public ICommand ConverterParaOsCommand { get; }

        // ===============================
        // CONTROLE DE EDIÇÃO
        // ===============================
        private int _orcamentoId;
        public bool Editando => _orcamentoId > 0;

        // ===============================
        // CONSTRUTOR NOVO
        // ===============================
        public OrcamentosViewModel()
        {
            Clientes = new ObservableCollection<Clientes>();

            SalvarOrcamentoCommand = new RelayCommand(
                SalvarOrcamento,
                PodeSalvarOrcamento
            );

            ImprimirOrcamentoCommand = new RelayCommand(ImprimirOrcamento);

            ConverterParaOsCommand = new RelayCommand(
                ConverterParaOrdemServico,
                () => Editando
            );

            RegistrarEventosColecoes();
        }

        // ===============================
        // CONSTRUTOR EDIÇÃO
        // ===============================
        public OrcamentosViewModel(Orcamentos orcamento) : this()
        {
            if (orcamento == null)
                return;

            _orcamentoId = orcamento.Id;

            // CLIENTE
            if (orcamento.ClienteId.HasValue)
            {
                using var db = new AppDbContext();
                var cliente = db.Clientes
                    .FirstOrDefault(c => c.Id == orcamento.ClienteId.Value);

                if (cliente != null)
                    PreencherClienteEmModoEdicao(cliente);
            }
            else
            {
                TextoClienteDigitado = orcamento.ClienteNome;
            }

            // VEÍCULO
            Veiculo = orcamento.Veiculo;
            Placa = orcamento.Placa;

            // LIMPA COLEÇÕES
            LimparColecoes();

            // CARREGA ITENS
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

                if (item.IsPeca)
                    AdicionarPeca(novo);
                else
                    AdicionarServico(novo);
            }

            AtualizarEstadoSalvar();
        }

        // ===============================
        // MÉTODOS AUXILIARES
        // ===============================
        private void RegistrarEventosColecoes()
        {
            foreach (var col in TodasColecoes())
            {
                col.CollectionChanged += (_, __) =>
                {
                    OnPropertyChanged(nameof(TotalServicos));
                    OnPropertyChanged(nameof(TotalPecas));
                    OnPropertyChanged(nameof(TotalGeral));
                    AtualizarEstadoSalvar();
                };
            }
        }

        private ObservableCollection<ItemOrdemServico>[] TodasColecoes() =>
            new[]
            {
                ItensBiela, ItensBloco, ItensCabecote, ItensEixo, ItensMotor,
                PecasBiela, PecasBloco, PecasCabecote, PecasEixo, PecasMotor
            };

        private void LimparColecoes()
        {
            foreach (var col in TodasColecoes())
                col.Clear();
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

        private System.Collections.Generic.List<ItemOrdemServico> ObterTodosItens()
        {
            return TodasColecoes()
                .SelectMany(c => c)
                .Where(i => i.Quantidade > 0 && !string.IsNullOrWhiteSpace(i.Servico))
                .ToList();
        }

        // ===============================
        // SALVAR ORÇAMENTO
        // ===============================
        private void SalvarOrcamento()
        {
            try
            {
                using var db = new AppDbContext();

                Orcamentos orcamento = _orcamentoId > 0
                    ? db.Orcamentos.Include(o => o.Itens).First(o => o.Id == _orcamentoId)
                    : new Orcamentos();

                orcamento.Itens.Clear();

                orcamento.ClienteId = ClienteSelecionado?.Id;
                orcamento.ClienteNome = ClienteSelecionado?.Nome ?? TextoClienteDigitado;
                orcamento.Veiculo = Veiculo;
                orcamento.Placa = Placa;
                orcamento.Data = DateTime.Now;

                var itensTela = ObterTodosItens();
                orcamento.Total = itensTela.Sum(i => i.Total);

                foreach (var item in itensTela)
                {
                    orcamento.Itens.Add(new ItemOrcamento
                    {
                        Servico = item.Servico,
                        Quantidade = item.Quantidade,
                        ValorUnitario = item.ValorUnitario,
                        Bloco = item.Bloco,
                        IsPeca = item.IsPeca
                    });
                }

                if (_orcamentoId == 0)
                    db.Orcamentos.Add(orcamento);

                db.SaveChanges();
                _orcamentoId = orcamento.Id;

                var pdf = new OrcamentoPdf(orcamento);

                var caminhoPdf = Path.Combine(
                    @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                    "PDFs",
                    "Orcamentos",
                    orcamento.Data.Year.ToString(),
                    orcamento.Data.Month.ToString("D2"),
                    $"ORCAMENTO_{orcamento.Id}.pdf"
                );

                Directory.CreateDirectory(Path.GetDirectoryName(caminhoPdf));
                pdf.GeneratePdf(caminhoPdf);


                MessageBox.Show("Orçamento salvo com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar orçamento:\n" + ex.Message);
            }
        }

        private bool PodeSalvarOrcamento()
        {
            return !string.IsNullOrWhiteSpace(Veiculo)
                && !string.IsNullOrWhiteSpace(Placa)
                && ObterTodosItens().Any();
        }

        private void AtualizarEstadoSalvar()
        {
            (SalvarOrcamentoCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ===============================
        // IMPRIMIR
        // ===============================
        private void ImprimirOrcamento()
        {
            MessageBox.Show("Impressão realizada via PDF.");
        }

        // ===============================
        // CONVERTER PARA OS
        // ===============================
        private void ConverterParaOrdemServico()
        {
            try
            {
                using var db = new AppDbContext();
                var orcamento = db.Orcamentos
                    .Include(o => o.Itens)
                    .First(o => o.Id == _orcamentoId);

                new OrcamentoService().ConverterEmOsEExcluir(orcamento);

                MessageBox.Show("Ordem de Serviço criada com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ===============================
        // INotifyPropertyChanged
        // ===============================
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
