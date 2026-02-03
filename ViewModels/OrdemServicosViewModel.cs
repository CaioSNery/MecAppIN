using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Enums;
using MecAppIN.Helpers;
using MecAppIN.Models;
using MecAppIN.Pdf;
using MecAppIN.Pdfs;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;


namespace MecAppIN.ViewModels
{
    public class OrdemServicosViewModel : ClienteBaseViewModel
    {
        public string Veiculo { get; set; }
        public string Placa { get; set; }

        public ObservableCollection<ItemOrdemServico> Itens { get; set; }
            = new();

        public ObservableCollection<string> TiposMotor { get; set; }

        private string _tipoMotorSelecionado;
        public string TipoMotorSelecionado
        {
            get => _tipoMotorSelecionado;
            set
            {
                if (_tipoMotorSelecionado == value)
                    return;

                _tipoMotorSelecionado = value;
                OnPropertyChanged();
                AtualizarItensPorMotor();
            }
        }

        private int _numeroOs;
        public int NumeroOs
        {
            get => _numeroOs;
            set
            {
                _numeroOs = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Editando));

                (AtualizarOsCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }



        public bool Editando => NumeroOs > 0;


        public ObservableCollection<ItemOrdemServico> ItensBiela { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> ItensBloco { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> ItensCabecote { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> ItensEixo { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> ItensMotor { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> PecasBiela { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> PecasBloco { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> PecasCabecote { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> PecasEixo { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> PecasMotor { get; set; } = new();





        public decimal TotalGeral =>
      // SERVIÇOS
      ItensBiela.Sum(i => i.Total)
    + ItensBloco.Sum(i => i.Total)
    + ItensCabecote.Sum(i => i.Total)
    + ItensEixo.Sum(i => i.Total)
    + ItensMotor.Sum(i => i.Total)

    // PEÇAS
    + PecasBiela.Sum(i => i.Total)
    + PecasBloco.Sum(i => i.Total)
    + PecasCabecote.Sum(i => i.Total)
    + PecasEixo.Sum(i => i.Total)
    + PecasMotor.Sum(i => i.Total);
        // TOTAL PARCIAL DE TODOS OS SERVIÇOS
        public decimal TotalServicos =>
            ItensBiela.Sum(i => i.Total)
          + ItensBloco.Sum(i => i.Total)
          + ItensCabecote.Sum(i => i.Total)
          + ItensEixo.Sum(i => i.Total)
          + ItensMotor.Sum(i => i.Total);

        // TOTAL PARCIAL DE TODAS AS PEÇAS
        public decimal TotalPecas =>
            PecasBiela.Sum(i => i.Total)
          + PecasBloco.Sum(i => i.Total)
          + PecasCabecote.Sum(i => i.Total)
          + PecasEixo.Sum(i => i.Total)
          + PecasMotor.Sum(i => i.Total);



        public ICommand AtualizarOsCommand { get; }
        public ICommand SalvarComoOrcamentoCommand { get; }
        public ICommand ImprimirOsCommand { get; }

        // ===============================
        // CONSTRUTOR 
        // ===============================

        public OrdemServicosViewModel(int ordemServicoId)
    : this()
        {
            CarregarOrdem(ordemServicoId);
        }

        public OrdemServicosViewModel()
        {
            TiposMotor = new ObservableCollection<string>
    {
        "Gasolina",
        "Diesel"
    };

            TipoMotorSelecionado = "Gasolina";

            CarregarItensBiela();
            CarregarItensBloco();
            CarregarItensCabecote();
            CarregarItensEixo();
            CarregarItensMotor();

            RegistrarEventos(ItensBiela);
            RegistrarEventos(ItensBloco);
            RegistrarEventos(ItensCabecote);
            RegistrarEventos(ItensEixo);
            RegistrarEventos(ItensMotor);



            RegistrarEventos(PecasBiela);
            RegistrarEventos(PecasBloco);
            RegistrarEventos(PecasCabecote);
            RegistrarEventos(PecasEixo);
            RegistrarEventos(PecasMotor);



            ImprimirOsCommand = new RelayCommand(ImprimirOs);
            AtualizarOsCommand = new RelayCommand(AtualizarOs, () => Editando);
            SalvarComoOrcamentoCommand = new RelayCommand(
            SalvarComoOrcamento,
            PodeSalvarComoOrcamento
            );
        }


        private bool PodeSalvarComoOrcamento()
        {
            // só permite se tiver itens e NÃO estiver editando uma OS
            return !Editando && ObterTodosItens().Any();
        }



        private void AtualizarOs()
        {
            try
            {
                var itensTela = ObterTodosItens();

                if (!itensTela.Any())
                {
                    MessageBox.Show("A OS não possui itens.");
                    return;
                }

                var itensBanco = ClonarItensParaPersistencia(itensTela);
                bool temCliente = ClienteSelecionado != null;

                using var db = new AppDbContext();

                var os = db.OrdemServicos
                    .Include(o => o.Itens)
                    .First(o => o.Id == NumeroOs);

                os.ClienteId = temCliente ? ClienteSelecionado.Id : null;
                os.ClienteNome = temCliente ? ClienteSelecionado.Nome : TextoClienteDigitado;
                os.ClienteEndereco = temCliente ? ClienteSelecionado.Endereco : ClienteEndereco;
                os.ClienteTelefone = temCliente ? ClienteSelecionado.Telefone : ClienteTelefone;

                os.TipoMotor = TipoMotorSelecionado;
                os.Veiculo = Veiculo;
                os.Placa = Placa;
                os.Total = TotalGeral;

                db.ItensOrdensServicos.RemoveRange(os.Itens);
                os.Itens = itensBanco;

                db.SaveChanges();

                GerarPdfInterno(os);

                MessageBox.Show(
                    "OS atualizada com sucesso.\nPDF atualizado.",
                    "Atualização concluída",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao atualizar OS:\n{ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }




        private void CarregarOrdem(int id)
        {
            using var db = new AppDbContext();

            var os = db.OrdemServicos
                .Include(o => o.Itens)
                .First(o => o.Id == id);

            // ===============================
            // DADOS PRINCIPAIS
            // ===============================
            NumeroOs = os.Id;
            Veiculo = os.Veiculo;
            Placa = os.Placa;
            TextoClienteDigitado = os.ClienteNome;
            TipoMotorSelecionado = os.TipoMotor;

            // ===============================
            // GARANTE SERVIÇOS PADRÃO
            // (se já existir, não duplica)
            // ===============================
            GarantirServicosPadrao(ItensBiela, CarregarItensBiela);
            GarantirServicosPadrao(ItensBloco, CarregarItensBloco);
            GarantirServicosPadrao(ItensCabecote, CarregarItensCabecote);
            GarantirServicosPadrao(ItensEixo, CarregarItensEixo);
            GarantirServicosPadrao(ItensMotor, CarregarItensMotor);

            // ===============================
            // APLICA ITENS DA OS
            // ===============================
            foreach (var item in os.Itens)
            {
                if (item.IsPeca)
                {
                    var destino = ObterListaPecasPorBloco(item.Bloco);

                    destino.Add(new ItemOrdemServico
                    {
                        Bloco = item.Bloco,
                        Servico = item.Servico,
                        Quantidade = item.Quantidade,
                        ValorUnitario = item.ValorUnitario,
                        IsPeca = true
                    });

                    continue;
                }

                // ===== SERVIÇO =====
                var lista = ObterListaPorBloco(item.Bloco);

                var servico = lista.FirstOrDefault(s =>
                    s.Servico == item.Servico);

                if (servico != null)
                {
                    servico.Quantidade = item.Quantidade;
                    servico.ValorUnitario = item.ValorUnitario;
                }
            }

            OnPropertyChanged(nameof(TotalServicos));
            OnPropertyChanged(nameof(TotalPecas));
            OnPropertyChanged(nameof(TotalGeral));
        }
        private ObservableCollection<ItemOrdemServico> ObterListaPecasPorBloco(EBlocoMotor bloco)
        {
            return bloco switch
            {
                EBlocoMotor.Biela => PecasBiela,
                EBlocoMotor.Bloco => PecasBloco,
                EBlocoMotor.Cabecote => PecasCabecote,
                EBlocoMotor.Eixo => PecasEixo,
                EBlocoMotor.Motor => PecasMotor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        private void GarantirServicosPadrao(
            ObservableCollection<ItemOrdemServico> lista,
            Action carregarPadrao)
        {
            if (lista.Count == 0)
                carregarPadrao();
        }
        private ObservableCollection<ItemOrdemServico> ObterListaPorBloco(EBlocoMotor bloco)
        {
            return bloco switch
            {
                EBlocoMotor.Biela => ItensBiela,
                EBlocoMotor.Bloco => ItensBloco,
                EBlocoMotor.Cabecote => ItensCabecote,
                EBlocoMotor.Eixo => ItensEixo,
                EBlocoMotor.Motor => ItensMotor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }


        private void LimparColecoes()
        {
            ItensBiela.Clear();
            ItensBloco.Clear();
            ItensCabecote.Clear();
            ItensEixo.Clear();
            ItensMotor.Clear();

            PecasBiela.Clear();
            PecasBloco.Clear();
            PecasCabecote.Clear();
            PecasEixo.Clear();
            PecasMotor.Clear();
        }


        private void RegistrarEventos(ObservableCollection<ItemOrdemServico> itens)
        {
            foreach (var item in itens)
                item.PropertyChanged += Item_PropertyChanged;

            itens.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (ItemOrdemServico item in e.NewItems)
                        item.PropertyChanged += Item_PropertyChanged;

                if (e.OldItems != null)
                    foreach (ItemOrdemServico item in e.OldItems)
                        item.PropertyChanged -= Item_PropertyChanged;

                OnPropertyChanged(nameof(TotalGeral));


                (SalvarComoOrcamentoCommand as RelayCommand)
                    ?.RaiseCanExecuteChanged();
            };
        }


        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemOrdemServico.Total) ||
                e.PropertyName == nameof(ItemOrdemServico.Quantidade) ||
                e.PropertyName == nameof(ItemOrdemServico.Servico))
            {
                OnPropertyChanged(nameof(TotalServicos));
                OnPropertyChanged(nameof(TotalPecas));
                OnPropertyChanged(nameof(TotalGeral));
                (SalvarComoOrcamentoCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
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
        // ITENS POR MOTOR
        // ===============================
        private readonly List<ItemOrdemServico> ItensGasolina = new()
        {
            new ItemOrdemServico { Servico = "Esmerilhar cabeçote", Quantidade = 8, ValorUnitario = 30 },
            new ItemOrdemServico { Servico = "Retífica bloco", Quantidade = 1, ValorUnitario = 500 }
        };

        private readonly List<ItemOrdemServico> ItensDiesel = new()
        {
            new ItemOrdemServico { Servico = "Esmerilhar cabeçote", Quantidade = 8, ValorUnitario = 45 },
            new ItemOrdemServico { Servico = "Retífica bloco", Quantidade = 1, ValorUnitario = 800 }
        };

        private void AtualizarItensPorMotor()
        {
            Itens.Clear();

            var lista = TipoMotorSelecionado == "Diesel"
                ? ItensDiesel
                : ItensGasolina;

            foreach (var item in lista)
            {
                Itens.Add(new ItemOrdemServico
                {
                    Servico = item.Servico,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario
                });
            }
        }


        private List<ItemOrdemServico> ObterTodosItens()
        {
            return ItensBiela
                .Concat(ItensBloco)
                .Concat(ItensCabecote)
                .Concat(ItensEixo)
                .Concat(ItensMotor)
                .Concat(PecasBiela)
                .Concat(PecasBloco)
                .Concat(PecasCabecote)
                .Concat(PecasEixo)
                .Concat(PecasMotor)
                .Where(i => i.Quantidade > 0 && !string.IsNullOrWhiteSpace(i.Servico))
                .ToList();
        }


        private List<ItemOrdemServico> ClonarItensParaPersistencia(List<ItemOrdemServico> itens)
        {
            return itens.Select(i => new ItemOrdemServico
            {
                Bloco = i.Bloco,
                Servico = i.Servico,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                IsPeca = i.IsPeca
            }).ToList();

        }

        // ===============================
        // IMPRIMIR
        // ===============================

        private void ImprimirOs()
        {
            try
            {
                var itensTela = ObterTodosItens();

                if (!itensTela.Any())
                {
                    MessageBox.Show("A OS não possui itens.");
                    return;
                }

                var itensBanco = ClonarItensParaPersistencia(itensTela);
                bool temCliente = ClienteSelecionado != null;

                using var db = new AppDbContext();
                OrdemServicos os;

                if (NumeroOs == 0)
                {
                    NumeroOs = NumeroOsService.Gerar(TipoMotorSelecionado);

                    os = new OrdemServicos
                    {
                        Id = NumeroOs, //  CONTROLE MANUAL
                        Data = DateTime.Now,
                        ClienteId = temCliente ? ClienteSelecionado.Id : null,
                        ClienteNome = temCliente ? ClienteSelecionado.Nome : TextoClienteDigitado,
                        ClienteEndereco = temCliente ? ClienteSelecionado.Endereco : ClienteEndereco,
                        ClienteTelefone = temCliente ? ClienteSelecionado.Telefone : ClienteTelefone,
                        Veiculo = Veiculo,
                        Placa = Placa,
                        TipoMotor = TipoMotorSelecionado,
                        Total = TotalGeral,
                        Itens = itensBanco
                    };

                    db.OrdemServicos.Add(os);
                    db.SaveChanges();
                }

                else
                {
                    os = db.OrdemServicos
                        .Include(o => o.Itens)
                        .First(o => o.Id == NumeroOs);

                    os.ClienteId = temCliente ? ClienteSelecionado.Id : null;
                    os.ClienteNome = temCliente ? ClienteSelecionado.Nome : TextoClienteDigitado;
                    os.ClienteEndereco = temCliente ? ClienteSelecionado.Endereco : ClienteEndereco;
                    os.ClienteTelefone = temCliente ? ClienteSelecionado.Telefone : ClienteTelefone;

                    os.Veiculo = Veiculo;
                    os.Placa = Placa;
                    os.TipoMotor = TipoMotorSelecionado;
                    os.Total = TotalGeral;

                    db.ItensOrdensServicos.RemoveRange(os.Itens);
                    os.Itens = itensBanco;

                    db.SaveChanges();
                }

                LimparColecoes();

                var caminhoPdf = GerarPdfInterno(os);
                PdfService.ImprimirPdfSeguro(caminhoPdf);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao imprimir OS:\n{ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        private void SalvarComoOrcamento()
        {
            var itensTela = ObterTodosItens();

            if (!itensTela.Any())
            {
                MessageBox.Show("Não há itens para salvar no orçamento.");
                return;
            }

            using var db = new AppDbContext();

            var orcamento = new Orcamentos
            {
                Data = DateTime.Now,
                ClienteId = ClienteSelecionado?.Id,
                ClienteNome = ClienteSelecionado != null
                    ? ClienteSelecionado.Nome
                    : TextoClienteDigitado,
                Veiculo = Veiculo,
                Placa = Placa,
                ClienteEndereco = ClienteSelecionado != null
                ? ClienteSelecionado.Endereco
                : ClienteEndereco,
                ClienteTelefone = ClienteSelecionado != null
                ? ClienteSelecionado.Telefone
                : ClienteTelefone,
                Total = TotalGeral,
                TipoMotor = TipoMotorSelecionado,
                NumeroOs = 0,
                Itens = itensTela.Select(i => new ItemOrcamento
                {
                    Servico = i.Servico,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    Bloco = i.Bloco,
                    IsPeca = i.IsPeca
                }).ToList()
            };

            db.Orcamentos.Add(orcamento);
            db.SaveChanges();
            NumeroOs = 0;
            LimparColecoes();
            Veiculo = string.Empty;
            Placa = string.Empty;
            TextoClienteDigitado = string.Empty;

            OnPropertyChanged(nameof(Veiculo));
            OnPropertyChanged(nameof(Placa));
            OnPropertyChanged(nameof(TextoClienteDigitado));

            GerarPdfOrcamento(orcamento);

            MessageBox.Show("Orçamento salvo com sucesso!");
        }

        public static class NumeroOsService
        {
            public static int Gerar(string tipoMotor)
            {
                using var db = new AppDbContext();

                var seq = db.SequenciasOs.Single(s => s.TipoMotor == tipoMotor);

                seq.UltimoNumero++;
                db.SaveChanges();

                return seq.UltimoNumero;
            }
        }





        private void GerarPdfOrcamento(Orcamentos orcamento)
        {
            var caminho = PdfPathHelper.ObterCaminhoOrcamento(orcamento);

            Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);

            new OrcamentoPdf(orcamento).GeneratePdf(caminho);
        }



        private string GerarPdfInterno(OrdemServicos os)
        {
            var caminhoPdf = PdfPathHelper.ObterCaminhoOs(os);

            Directory.CreateDirectory(Path.GetDirectoryName(caminhoPdf)!);

            var pdf = new OrdemServicoPdf(os);
            pdf.GeneratePdf(caminhoPdf);

            return caminhoPdf;
        }


    }
}
