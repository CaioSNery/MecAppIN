using MecAppIN.Commands;
using MecAppIN.Models;
using System.Collections.ObjectModel;
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

        // ===============================
        // IMPRIMIR
        // ===============================
        private void ImprimirOs()
        {
            MessageBox.Show("Imprimir Ordem de Serviço");
        }
    }
}
