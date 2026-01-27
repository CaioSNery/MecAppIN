using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Enums;
using MecAppIN.Models;
using MecAppIN.Pdf;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Xps.Packaging;

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

                using var db = new AppDbContext();

                var os = db.OrdemServicos
                    .Include(o => o.Cliente)
                    .Include(o => o.Itens)
                    .First(o => o.Id == NumeroOs);

                // ===============================
                // ATUALIZA DADOS PRINCIPAIS
                // ===============================
                os.ClienteId = ClienteSelecionado?.Id;
                os.ClienteNome = TextoClienteDigitado;
                os.TipoMotor = TipoMotorSelecionado;
                os.Veiculo = Veiculo;
                os.Placa = Placa;
                os.Total = TotalGeral;

                // ===============================
                // ATUALIZA ITENS
                // ===============================
                db.ItensOrdensServicos.RemoveRange(os.Itens);
                os.Itens = itensBanco;

                db.SaveChanges();

                // ===============================
                // 🔥 ENDEREÇO VEM DO CLIENTE
                // ===============================
                os.ClienteEndereco = os.Cliente?.Endereco ?? "";

                // ===============================
                // PDF ATUALIZADO
                // ===============================
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

            // Dados principais
            NumeroOs = os.Id;
            Veiculo = os.Veiculo;
            Placa = os.Placa;
            TextoClienteDigitado = os.ClienteNome;


            // Limpa tudo
            LimparColecoes();

            // Recarrega itens
            foreach (var item in os.Itens)
            {
                if (item.IsPeca)
                    AdicionarPeca(item);
                else
                    AdicionarServico(item);
            }

            OnPropertyChanged(nameof(TotalServicos));
            OnPropertyChanged(nameof(TotalPecas));
            OnPropertyChanged(nameof(TotalGeral));
        }

        private void AdicionarPeca(ItemOrdemServico item)
        {
            var nova = new ItemOrdemServico
            {
                Bloco = item.Bloco,
                Servico = item.Servico,
                Quantidade = item.Quantidade,
                ValorUnitario = item.ValorUnitario,
                IsPeca = true
            };

            switch (item.Bloco)
            {
                case EBlocoMotor.Biela:
                    PecasBiela.Add(nova);
                    break;
                case EBlocoMotor.Bloco:
                    PecasBloco.Add(nova);
                    break;
                case EBlocoMotor.Cabecote:
                    PecasCabecote.Add(nova);
                    break;
                case EBlocoMotor.Eixo:
                    PecasEixo.Add(nova);
                    break;
                case EBlocoMotor.Motor:
                    PecasMotor.Add(nova);
                    break;
            }
        }


        private void AdicionarServico(ItemOrdemServico item)
        {
            var novo = new ItemOrdemServico
            {
                Bloco = item.Bloco,
                Servico = item.Servico,
                Quantidade = item.Quantidade,
                ValorUnitario = item.ValorUnitario,
                IsPeca = false,
                ValorEditavel = true
            };

            switch (item.Bloco)
            {
                case EBlocoMotor.Biela:
                    ItensBiela.Add(novo);
                    break;
                case EBlocoMotor.Bloco:
                    ItensBloco.Add(novo);
                    break;
                case EBlocoMotor.Cabecote:
                    ItensCabecote.Add(novo);
                    break;
                case EBlocoMotor.Eixo:
                    ItensEixo.Add(novo);
                    break;
                case EBlocoMotor.Motor:
                    ItensMotor.Add(novo);
                    break;
            }
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

                // 🔥 AVISA O BOTÃO
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

                // 🔥 ATUALIZA O BOTÃO
                (SalvarComoOrcamentoCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }




        private void CarregarItensBiela()
        {
            ItensBiela.Clear();

            ItensBiela.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                Servico = "Retificar",
                Quantidade = 0,
                ValorUnitario = 120,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensBiela.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                Servico = "Acoplar",
                Quantidade = 0,
                ValorUnitario = 50,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensBiela.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                Servico = "Embuchar",
                Quantidade = 0,
                ValorUnitario = 0,
                IsPeca = false,
                ValorEditavel = true // editável
            });


        }

        private void CarregarItensBloco()
        {
            ItensBloco.Clear();

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Retificar Cilindro",
                Quantidade = 0,
                ValorUnitario = 150,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Encamisar Cilindro",
                Quantidade = 0,
                ValorUnitario = 200,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Brunir Cilindro",
                Quantidade = 0,
                ValorUnitario = 50,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Mandrilhar Mancais",
                Quantidade = 0,
                ValorUnitario = 225,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Facear",
                Quantidade = 0,
                ValorUnitario = 400,
                IsPeca = false,
                ValorEditavel = true
            });


        }

        public void CarregarItensCabecote()
        {
            ItensCabecote.Clear();

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Retificar Sede",
                Quantidade = 0,
                ValorUnitario = 12.50M,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Retificar Válvulas",
                Quantidade = 0,
                ValorUnitario = 12.50M,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Descarb./Esmer./Montar",
                Quantidade = 0,
                ValorUnitario = 21.25M,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Facear",
                Quantidade = 0,
                ValorUnitario = 200,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Calibrar Válvulas",
                Quantidade = 0,
                ValorUnitario = 25,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Mandrilhar",
                Quantidade = 0,
                ValorUnitario = 350,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Substituir Guia",
                Quantidade = 0,
                ValorUnitario = 80,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Recondic./ Solda",
                Quantidade = 0,
                ValorUnitario = 30,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Extração de parafuso",
                Quantidade = 0,
                ValorUnitario = 70,
                IsPeca = false,
                ValorEditavel = true
            });

        }

        public void CarregarItensEixo()
        {
            ItensEixo.Clear();

            ItensEixo.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                Servico = "Retificar Eixo ",
                Quantidade = 0,
                ValorUnitario = 320,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensEixo.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                Servico = "Polir Eixo",
                Quantidade = 0,
                ValorUnitario = 140,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensEixo.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                Servico = "Polir Eixo de comando",
                Quantidade = 0,
                ValorUnitario = 80,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensEixo.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                Servico = "Recuperar lateral",
                Quantidade = 0,
                ValorUnitario = 180,
                IsPeca = false,
                ValorEditavel = true
            });



        }

        public void CarregarItensMotor()
        {

            ItensMotor.Clear();

            ItensMotor.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                Servico = "Descarbonizar Motor",
                Quantidade = 0,
                ValorUnitario = 100,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensMotor.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                Servico = "Montagem parcial",
                Quantidade = 0,
                ValorUnitario = 0,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensMotor.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                Servico = "Montagem completa",
                Quantidade = 0,
                ValorUnitario = 0,
                IsPeca = false,
                ValorEditavel = true
            });

            ItensMotor.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                Servico = "Serviço de Troca",
                Quantidade = 0,
                ValorUnitario = 0,
                IsPeca = false,
                ValorEditavel = true
            });


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

                using var db = new AppDbContext();
                OrdemServicos os;

                // ===============================
                // NOVA OS
                // ===============================
                if (NumeroOs == 0)
                {
                    os = new OrdemServicos
                    {
                        Data = DateTime.Now,
                        ClienteId = ClienteSelecionado?.Id,
                        ClienteNome = TextoClienteDigitado,

                        ClienteEndereco = ClienteSelecionado != null
                            ? ClienteSelecionado.Endereco
                            : string.Empty,

                        ClienteTelefone = ClienteSelecionado != null
                            ? ClienteSelecionado.Telefone
                            : string.Empty,

                        Veiculo = Veiculo,
                        Placa = Placa,
                        TipoMotor = TipoMotorSelecionado,
                        Total = TotalGeral,
                        Itens = itensBanco
                    };

                    db.OrdemServicos.Add(os);
                    db.SaveChanges();
                }
                // ===============================
                // EDIÇÃO DE OS EXISTENTE
                // ===============================
                else
                {
                    os = db.OrdemServicos
                        .Include(o => o.Cliente)
                        .Include(o => o.Itens)
                        .First(o => o.Id == NumeroOs);

                    os.ClienteId = ClienteSelecionado?.Id;
                    os.ClienteNome = TextoClienteDigitado;

                    // Atualiza endereço e telefone SOMENTE se houver cliente selecionado
                    if (ClienteSelecionado != null)
                    {
                        os.ClienteEndereco = ClienteSelecionado.Endereco;
                        os.ClienteTelefone = ClienteSelecionado.Telefone;
                    }

                    os.Veiculo = Veiculo;
                    os.TipoMotor = TipoMotorSelecionado;
                    os.Placa = Placa;
                    os.Total = TotalGeral;

                    db.ItensOrdensServicos.RemoveRange(os.Itens);
                    os.Itens = itensBanco;

                    db.SaveChanges();
                }

                // ===============================
                // PDF + IMPRESSÃO
                // ===============================
                GerarPdfInterno(os);
                ImprimirOsComDialogo(os);
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


        private void ImprimirOsComDialogo(OrdemServicos os)
        {
            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true)
                return;

            var tempXps = Path.Combine(
                Path.GetTempPath(),
                $"OS_{os.Id}.xps"
            );

            try
            {
                var pdf = new OrdemServicoPdf(os);
                pdf.GenerateXps(tempXps);

                using var xpsDoc = new XpsDocument(tempXps, FileAccess.Read);
                var paginator = xpsDoc.GetFixedDocumentSequence().DocumentPaginator;

                printDialog.PrintDocument(
                    paginator,
                    $"Ordem de Serviço #{os.Id}"
                );
            }
            finally
            {
                if (File.Exists(tempXps))
                    File.Delete(tempXps);
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
                Total = TotalGeral,
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




        private void GerarPdfOrcamento(Orcamentos orcamento)
        {
            var basePath = Path.Combine(
                @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                "PDFs",
                "Orcamentos",
                orcamento.Data.Year.ToString(),
                orcamento.Data.Month.ToString("D2")
            );

            Directory.CreateDirectory(basePath);

            var caminho = Path.Combine(
                basePath,
                $"ORCAMENTO_{orcamento.Id}.pdf"
            );

            var pdf = new OrcamentoPdf(orcamento);
            pdf.GeneratePdf(caminho);
        }





        private string GerarPdfInterno(OrdemServicos os)
        {
            var basePath = Path.Combine(
                @"C:\Users\USER\Desktop\Projetos\MecAppIN",
                "PDFs",
                "OrdensDeServico",
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString("D2")
            );

            Directory.CreateDirectory(basePath);

            var caminhoPdf = Path.Combine(
                basePath,
                $"OS_{os.Id}.pdf"
            );

            var pdf = new OrdemServicoPdf(os);
            pdf.GeneratePdf(caminhoPdf);

            return caminhoPdf;
        }

    }
}
