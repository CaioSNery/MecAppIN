using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;
using System.Collections.ObjectModel;
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

            ImprimirOsCommand = new RelayCommand(ImprimirOs);
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



        // ===============================
        // IMPRIMIR
        // ===============================
        private void ImprimirOs()
        {
            if (Itens == null || !Itens.Any())
            {
                MessageBox.Show("A Ordem de Serviço não possui itens para impressão.",
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
                Text = "ORDEM DE SERVIÇO",
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

            if (printDialog.ShowDialog() != true)
                return;

            // SALVA A OS
            SalvarOrdemServico();

            // IMPRIME
            printDialog.PrintVisual(painel, "Impressão de Ordem de Serviço");

        }
    }
}
