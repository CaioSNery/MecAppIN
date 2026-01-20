using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Enums;
using MecAppIN.Models;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
            }
        }

        public ObservableCollection<ItemOrdemServico> ItensBiela { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> ItensBloco { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> ItensCabecote { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> ItensEixo { get; set; } = new();
        public ObservableCollection<ItemOrdemServico> ItensMotor { get; set; } = new();


        public decimal TotalGeral =>
    ItensBiela.Sum(i => i.Total)
  + ItensBloco.Sum(i => i.Total)
  + ItensCabecote.Sum(i => i.Total)
  + ItensEixo.Sum(i => i.Total)
  + ItensMotor.Sum(i => i.Total);




        public ICommand ImprimirOsCommand { get; }

        // ===============================
        // CONSTRUTOR (ÚNICO)
        // ===============================
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


            ImprimirOsCommand = new RelayCommand(ImprimirOs);
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
            };
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemOrdemServico.Total))
                OnPropertyChanged(nameof(TotalGeral));
        }


        private void CarregarItensBiela()
        {
            ItensBiela.Clear();

            ItensBiela.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                Servico = "Retificar",
                Quantidade = 0,
                ValorUnitario = 120
            });

            ItensBiela.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                Servico = "Acoplar",
                Quantidade = 0,
                ValorUnitario = 50
            });

            ItensBiela.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                Servico = "Embuchar",
                Quantidade = 0,
                ValorUnitario = 0 // editável
            });

            // 2 linhas em branco
            ItensBiela.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Biela });
            ItensBiela.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Biela });
        }

        private void CarregarItensBloco()
        {
            ItensBloco.Clear();

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Retificar Cilindro",
                Quantidade = 0,
                ValorUnitario = 150
            });

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Encamisar Cilindro",
                Quantidade = 0,
                ValorUnitario = 200
            });

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Brunir Cilindro",
                Quantidade = 0,
                ValorUnitario = 50
            });

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Mandrilhar Mancais",
                Quantidade = 0,
                ValorUnitario = 225
            });

            ItensBloco.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                Servico = "Facear",
                Quantidade = 0,
                ValorUnitario = 400
            });

            // 4 linhas em branco
            ItensBloco.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Bloco });
            ItensBloco.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Bloco });
            ItensBloco.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Bloco });
            ItensBloco.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Bloco });
        }

        public void CarregarItensCabecote()
        {
            ItensCabecote.Clear();

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Retificar Sede",
                Quantidade = 0,
                ValorUnitario = 12.50M
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Retificar Válvulas",
                Quantidade = 0,
                ValorUnitario = 12.50M
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Descarb./Esmer./Montar",
                Quantidade = 0,
                ValorUnitario = 21.25M
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Facear",
                Quantidade = 0,
                ValorUnitario = 200
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Calibrar Válvulas",
                Quantidade = 0,
                ValorUnitario = 25
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Mandrilhar",
                Quantidade = 0,
                ValorUnitario = 350
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Substituir Guia",
                Quantidade = 0,
                ValorUnitario = 80
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Recondic./ Solda",
                Quantidade = 0,
                ValorUnitario = 30
            });

            ItensCabecote.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                Servico = "Extração de parafuso",
                Quantidade = 0,
                ValorUnitario = 70
            });


            ItensCabecote.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote });
            ItensCabecote.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote });
            ItensCabecote.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote });
            ItensCabecote.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote });
            ItensCabecote.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Cabecote });

        }

        public void CarregarItensEixo()
        {
            ItensEixo.Clear();

            ItensEixo.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                Servico = "Retificar Eixo ",
                Quantidade = 0,
                ValorUnitario = 320
            });

            ItensEixo.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                Servico = "Polir Eixo",
                Quantidade = 0,
                ValorUnitario = 140
            });

            ItensEixo.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                Servico = "Polir Eixo de comando",
                Quantidade = 0,
                ValorUnitario = 80
            });

            ItensEixo.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                Servico = "Recuperar lateral",
                Quantidade = 0,
                ValorUnitario = 180
            });

            ItensEixo.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Eixo });
            ItensEixo.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Eixo });

        }

        public void CarregarItensMotor()
        {

            ItensMotor.Clear();

            ItensMotor.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                Servico = "Descarbonizar Motor",
                Quantidade = 0,
                ValorUnitario = 100
            });

            ItensMotor.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                Servico = "Montagem parcial",
                Quantidade = 0,
                ValorUnitario = 0
            });

            ItensMotor.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                Servico = "Montagem completa",
                Quantidade = 0,
                ValorUnitario = 0
            });

            ItensMotor.Add(new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                Servico = "Serviço de Troca",
                Quantidade = 0,
                ValorUnitario = 0
            });

            ItensMotor.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Motor });
            ItensMotor.Add(new ItemOrdemServico { Bloco = EBlocoMotor.Motor });
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

        private void SalvarOrdemServico()
        {
            using var context = new AppDbContext();

            var ordem = new OrdemServicos
            {
                Data = DateTime.Now,
                ClienteId = ClienteSelecionado?.Id,
                ClienteNome = TextoClienteDigitado,
                Veiculo = Veiculo,
                Placa = Placa,
                Total = Itens.Sum(i => i.Quantidade * i.ValorUnitario)
            };

            foreach (var item in Itens)
            {
                ordem.Itens.Add(new ItemOrdemServico
                {
                    Servico = item.Servico,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario
                });
            }

            context.OrdemServicos.Add(ordem);
            context.SaveChanges();
        }

        private List<ItemOrdemServico> ObterTodosItens()
        {
            return ItensBiela
                .Concat(ItensBloco)
                .Concat(ItensCabecote)
                .Concat(ItensEixo)
                .Concat(ItensMotor)
                .Where(i => i.Quantidade > 0 && i.ValorUnitario > 0)
                .ToList();
        }

        private List<ItemOrdemServico> ClonarItensParaPersistencia(List<ItemOrdemServico> itens)
        {
            return itens.Select(i => new ItemOrdemServico
            {
                Bloco = i.Bloco,
                Servico = i.Servico,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario
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

                var os = new OrdemServicos
                {
                    Data = DateTime.Now,
                    ClienteId = ClienteSelecionado?.Id,
                    ClienteNome = TextoClienteDigitado,
                    Veiculo = Veiculo,
                    Placa = Placa,
                    Itens = itensBanco,
                    Total = itensBanco.Sum(i => i.Total)
                };

                using (var db = new AppDbContext())
                {
                    db.OrdemServicos.Add(os);
                    db.SaveChanges();
                }

                var pdf = new OrdemServicoPdf(os);

                var caminho = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"OS_{os.Id}.pdf"
                );

                pdf.GeneratePdf(caminho);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = caminho,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao gerar OS:\n{ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }



    }
}
