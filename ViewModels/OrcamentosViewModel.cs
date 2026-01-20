using ClosedXML.Excel;
using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MecAppIN.ViewModels
{
    public class OrcamentosViewModel : ClienteBaseViewModel
    {
        // ===============================
        // CABEÇALHO DO ORÇAMENTO
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
        // ITENS DO ORÇAMENTO
        // ===============================
        public ObservableCollection<ItemOrcamento> Itens { get; set; }

        // ===============================
        // COMMANDS
        // ===============================
        public ICommand GerarExcelCommand { get; }
        public ICommand SalvarOrcamentoCommand { get; }
        public ICommand ImprimirOrcamentoCommand { get; }

        public ICommand ConverterParaOsCommand { get; }



        // ===============================
        // CONSTRUTOR
        // ===============================
        public OrcamentosViewModel()
        {
            Clientes = new ObservableCollection<Clientes>();
            Itens = new ObservableCollection<ItemOrcamento>();

            Itens.CollectionChanged += (s, e) => AtualizarEstadoSalvar();

            TiposMotor = new ObservableCollection<string>
            {
                "Gasolina",
                "Diesel"
            };

            TipoMotorSelecionado = "Gasolina";

            AtualizarItensPorMotor();

            SalvarOrcamentoCommand = new RelayCommand(
                SalvarOrcamento,
                PodeSalvarOrcamento
            );

            GerarExcelCommand = new RelayCommand(GerarExcel);
            ImprimirOrcamentoCommand = new RelayCommand(ImprimirOrcamento);

            ConverterParaOsCommand = new RelayCommand(ConverterParaOrdemServico);


        }

        // ===============================
        // TIPO DE MOTOR
        // ===============================
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

        private readonly List<ItemOrcamento> ItensGasolina = new()
        {
            new ItemOrcamento { Servico = "Esmerilhar cabeçote", Quantidade = 8, ValorUnitario = 30 },
            new ItemOrcamento { Servico = "Retífica bloco", Quantidade = 1, ValorUnitario = 500 }
        };

        private readonly List<ItemOrcamento> ItensDiesel = new()
        {
            new ItemOrcamento { Servico = "Esmerilhar cabeçote", Quantidade = 8, ValorUnitario = 45 },
            new ItemOrcamento { Servico = "Retífica bloco", Quantidade = 1, ValorUnitario = 800 }
        };

        private void AtualizarItensPorMotor()
        {
            Itens.Clear();

            var lista = TipoMotorSelecionado == "Diesel"
                ? ItensDiesel
                : ItensGasolina;

            foreach (var item in lista)
            {
                Itens.Add(new ItemOrcamento
                {
                    Servico = item.Servico,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario
                });
            }
        }

        private void ImprimirOrcamento()
        {
            if (Itens == null || !Itens.Any())
            {
                MessageBox.Show("O orçamento não possui itens para impressão.",
                                "Atenção",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true)
                return;

            // ===============================
            // LAYOUT DE IMPRESSÃO
            // ===============================
            var painel = new StackPanel
            {
                Margin = new Thickness(30),
                Width = printDialog.PrintableAreaWidth
            };

            painel.Children.Add(new TextBlock
            {
                Text = "ORÇAMENTO",
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            painel.Children.Add(new TextBlock { Text = $"Cliente: {TextoClienteDigitado}" });
            painel.Children.Add(new TextBlock { Text = $"Endereço: {ClienteEndereco}" });
            painel.Children.Add(new TextBlock { Text = $"Telefone: {ClienteTelefone}" });
            painel.Children.Add(new TextBlock { Text = $"Veículo: {Veiculo}" });
            painel.Children.Add(new TextBlock { Text = $"Placa: {Placa}" });
            painel.Children.Add(new TextBlock
            {
                Text = $"Data: {DateTime.Now:dd/MM/yyyy}",
                Margin = new Thickness(0, 0, 0, 15)
            });

            // Linha separadora
            painel.Children.Add(new Separator());

            // ===============================
            // TABELA DE ITENS
            // ===============================
            foreach (var item in Itens)
            {
                var linha = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                linha.Children.Add(new TextBlock
                {
                    Text = item.Servico,
                    Width = 250
                });

                linha.Children.Add(new TextBlock
                {
                    Text = item.Quantidade.ToString(),
                    Width = 60
                });

                linha.Children.Add(new TextBlock
                {
                    Text = item.ValorUnitario.ToString("C"),
                    Width = 100
                });

                linha.Children.Add(new TextBlock
                {
                    Text = (item.Quantidade * item.ValorUnitario).ToString("C"),
                    Width = 100
                });

                painel.Children.Add(linha);
            }

            painel.Children.Add(new Separator());

            painel.Children.Add(new TextBlock
            {
                Text = $"TOTAL: {Itens.Sum(i => i.Quantidade * i.ValorUnitario):C}",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            });

            // Mede e organiza o layout antes de imprimir
            painel.Measure(new System.Windows.Size(
            printDialog.PrintableAreaWidth,
            printDialog.PrintableAreaHeight));
            painel.Arrange(new Rect(new Point(0, 0), painel.DesiredSize));

            // ===============================
            // IMPRIMIR
            // ===============================
            printDialog.PrintVisual(painel, "Impressão de Orçamento");
        }


        private void GerarExcel()
        {
            try
            {
                var cliente = ObterOuCriarCliente();
                if (cliente == null)
                    return;

                // Validações básicas
                if (string.IsNullOrWhiteSpace(Veiculo) || string.IsNullOrWhiteSpace(Placa))
                {
                    MessageBox.Show("Informe o veículo e a placa antes de gerar o Excel.",
                                    "Atenção",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }

                if (Itens == null || !Itens.Any())
                {
                    MessageBox.Show("Adicione ao menos um serviço ao orçamento.",
                                    "Atenção",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }

                // Escolher onde salvar
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Arquivo Excel (*.xlsx)|*.xlsx",
                    FileName = $"Orcamento_{cliente.Nome}_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (dialog.ShowDialog() != true)
                    return;

                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Orçamento");

                // ===============================
                // CABEÇALHO
                // ===============================
                ws.Cell("A1").Value = "Cliente:";
                ws.Cell("B1").Value = cliente.Nome;

                ws.Cell("A2").Value = "Endereço:";
                ws.Cell("B2").Value = cliente.Endereco;

                ws.Cell("A3").Value = "Telefone:";
                ws.Cell("B3").Value = cliente.Telefone;

                ws.Cell("A4").Value = "Veículo:";
                ws.Cell("B4").Value = Veiculo;

                ws.Cell("A5").Value = "Placa:";
                ws.Cell("B5").Value = Placa;

                ws.Cell("A6").Value = "Data:";
                ws.Cell("B6").Value = DataOrcamento.ToShortDateString();

                // ===============================
                // TABELA
                // ===============================
                ws.Cell("A8").Value = "Serviço";
                ws.Cell("B8").Value = "Qtde";
                ws.Cell("C8").Value = "Valor Unit";
                ws.Cell("D8").Value = "Total";

                int linha = 9;
                foreach (var item in Itens)
                {
                    ws.Cell(linha, 1).Value = item.Servico;
                    ws.Cell(linha, 2).Value = item.Quantidade;
                    ws.Cell(linha, 3).Value = item.ValorUnitario;
                    ws.Cell(linha, 4).FormulaA1 = $"=B{linha}*C{linha}";
                    linha++;
                }

                ws.Cell(linha + 1, 3).Value = "TOTAL GERAL:";
                ws.Cell(linha + 1, 4).FormulaA1 = $"=SUM(D9:D{linha - 1})";

                ws.Columns().AdjustToContents();

                // Salva
                wb.SaveAs(dialog.FileName);

                // Abre automaticamente
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = dialog.FileName,
                    UseShellExecute = true
                });

                MessageBox.Show("Excel gerado com sucesso!",
                                "Sucesso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao gerar Excel:\n\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }



        // ===============================
        // CLIENTE
        // ===============================
        private Clientes ObterOuCriarCliente()
        {
            using var db = new AppDbContext();

            if (ClienteSelecionado != null && ClienteSelecionado.Id > 0)
                return ClienteSelecionado;

            var existente = db.Clientes
                .FirstOrDefault(c => c.Nome == TextoClienteDigitado);

            if (existente != null)
                return existente;

            var resultado = MessageBox.Show(
                $"Cliente \"{TextoClienteDigitado}\" não encontrado.\nDeseja cadastrar?",
                "Novo cliente",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
                return null;

            var novo = new Clientes
            {
                Nome = TextoClienteDigitado,
                Endereco = ClienteEndereco,
                Telefone = ClienteTelefone
            };

            db.Clientes.Add(novo);
            db.SaveChanges();

            return novo;
        }

        // ===============================
        // SALVAR ORÇAMENTO
        // ===============================
        private void SalvarOrcamento()
        {
            try
            {
                using var db = new AppDbContext();

                Orcamentos orcamento;

                // ===============================
                // EDIÇÃO
                // ===============================
                if (_orcamentoId > 0)
                {
                    orcamento = db.Orcamentos
                        .Include(o => o.Itens)
                        .First(o => o.Id == _orcamentoId);

                    orcamento.Itens.Clear();
                }
                // ===============================
                // NOVO
                // ===============================
                else
                {
                    orcamento = new Orcamentos();
                    db.Orcamentos.Add(orcamento);
                }

                // ===============================
                // CLIENTE (OPCIONAL)
                // ===============================
                if (ClienteSelecionado != null && ClienteSelecionado.Id > 0)
                {
                    orcamento.ClienteId = ClienteSelecionado.Id;
                    orcamento.ClienteNome = ClienteSelecionado.Nome;
                }
                else
                {
                    orcamento.ClienteId = null;
                    orcamento.ClienteNome = TextoClienteDigitado;
                }


                // ===============================
                // DADOS DO ORÇAMENTO
                // ===============================
                orcamento.Veiculo = Veiculo?.Trim();
                orcamento.Placa = Placa?.Trim();
                orcamento.Data = DateTime.Now;
                orcamento.Total = Itens.Sum(i => i.Quantidade * i.ValorUnitario);

                // ===============================
                // ITENS
                // ===============================
                foreach (var item in Itens)
                {
                    orcamento.Itens.Add(new ItemOrcamento
                    {
                        Servico = item.Servico,
                        Quantidade = item.Quantidade,
                        ValorUnitario = item.ValorUnitario
                    });
                }

                db.SaveChanges();

                _orcamentoId = orcamento.Id;

                MessageBox.Show("Orçamento salvo com sucesso!",
                                "Sucesso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao salvar orçamento:\n\n" + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }



        private bool PodeSalvarOrcamento()
        {
            return !string.IsNullOrWhiteSpace(Veiculo)
                && !string.IsNullOrWhiteSpace(Placa)
                && Itens.Any();
        }

        private void AtualizarEstadoSalvar()
        {
            (SalvarOrcamentoCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ===============================
        // EDIÇÃO
        // ===============================
        private int _orcamentoId = 0;

        public OrcamentosViewModel(Orcamentos orcamento) : this()
        {
            if (orcamento == null)
                return;

            _orcamentoId = orcamento.Id;

            // ===============================
            // CLIENTE
            // ===============================
            if (orcamento.Cliente != null)
            {
                // Cliente cadastrado
                ClienteSelecionado = orcamento.Cliente;
                TextoClienteDigitado = orcamento.Cliente.Nome;
                ClienteEndereco = orcamento.Cliente.Endereco;
                ClienteTelefone = orcamento.Cliente.Telefone;
            }
            else
            {
                // Cliente não cadastrado
                ClienteSelecionado = null;
                TextoClienteDigitado = orcamento.ClienteNome;
                ClienteEndereco = string.Empty;
                ClienteTelefone = string.Empty;
            }


            // Cabeçalho
            Veiculo = orcamento.Veiculo;
            Placa = orcamento.Placa;
            DataOrcamento = orcamento.Data;

            // Itens
            Itens.Clear();
            foreach (var item in orcamento.Itens)
            {
                Itens.Add(new ItemOrcamento
                {
                    Servico = item.Servico,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario
                });
            }
        }

        private void ConverterParaOrdemServico()
        {
            try
            {
                if (Itens == null || !Itens.Any())
                {
                    MessageBox.Show("O orçamento não possui itens.",
                                    "Atenção",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }

                using var db = new AppDbContext();

                var os = new OrdemServicos
                {
                    Data = DateTime.Now,

                    // vínculo opcional com orçamento
                    OrcamentoId = _orcamentoId > 0 ? _orcamentoId : null,

                    // cliente
                    ClienteId = ClienteSelecionado?.Id,
                    Cliente = ClienteSelecionado,
                    ClienteNome = ClienteSelecionado != null
                        ? ClienteSelecionado.Nome
                        : TextoClienteDigitado,

                    // veículo
                    Veiculo = Veiculo,
                    Placa = Placa,

                    // total
                    Total = Itens.Sum(i => i.Quantidade * i.ValorUnitario)
                };

                foreach (var item in Itens)
                {
                    os.Itens.Add(new ItemOrdemServico
                    {
                        Servico = item.Servico,
                        Quantidade = item.Quantidade,
                        ValorUnitario = item.ValorUnitario
                    });
                }

                db.OrdemServicos.Add(os);
                db.SaveChanges();

                MessageBox.Show("Ordem de Serviço criada com sucesso!",
                                "Sucesso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // (opcional) navegar para a tela da OS depois de criar
                // AbrirTelaOrdemServico(os);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gerar Ordem de Serviço:\n\n" + ex.Message,
                                "Erro",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }


    



        // ===============================
        // INotifyPropertyChanged
        // ===============================
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
