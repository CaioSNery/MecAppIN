using ClosedXML.Excel;
using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MecAppIN.ViewModels
{
    public class OrcamentosViewModel : INotifyPropertyChanged
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
        // CLIENTES
        // ===============================
        public ObservableCollection<Clientes> Clientes { get; set; }

        public bool IsSelecionandoCliente { get; set; }

        private Clientes _clienteSelecionado;
        public Clientes ClienteSelecionado
        {
            get => _clienteSelecionado;
            set
            {
                _clienteSelecionado = value;
                OnPropertyChanged();
                AtualizarDadosCliente();
            }
        }

        private string _textoClienteDigitado;
        public string TextoClienteDigitado
        {
            get => _textoClienteDigitado;
            set
            {
                if (_textoClienteDigitado == value)
                    return;

                _textoClienteDigitado = value;
                OnPropertyChanged();

                if (!IsSelecionandoCliente)
                    BuscarClientes();
            }
        }

        public string ClienteEndereco { get; set; }
        public string ClienteTelefone { get; set; }

        // ===============================
        // ITENS DO ORÇAMENTO
        // ===============================
        public ObservableCollection<ItemOrcamento> Itens { get; set; }

        // ===============================
        // COMMANDS
        // ===============================
        public ICommand GerarExcelCommand { get; }
        public ICommand SalvarOrcamentoCommand { get; }

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
        // BUSCAR CLIENTES
        // ===============================
        private void BuscarClientes()
        {
            using var db = new AppDbContext();

            Clientes.Clear();

            if (string.IsNullOrWhiteSpace(TextoClienteDigitado))
                return;

            var termo = TextoClienteDigitado.Trim();

            var lista = db.Clientes
                .Where(c => EF.Functions.Like(c.Nome, $"%{termo}%"))
                .OrderBy(c => c.Nome)
                .Take(20)
                .ToList();

            foreach (var c in lista)
                Clientes.Add(c);
        }

        private void AtualizarDadosCliente()
        {
            if (ClienteSelecionado == null)
                return;

            TextoClienteDigitado = ClienteSelecionado.Nome;
            ClienteEndereco = ClienteSelecionado.Endereco;
            ClienteTelefone = ClienteSelecionado.Telefone;

            OnPropertyChanged(nameof(ClienteEndereco));
            OnPropertyChanged(nameof(ClienteTelefone));
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

                var cliente = ObterOuCriarCliente();
                if (cliente == null)
                    return;

                Orcamentos orcamento;

                // ===============================
                // EDIÇÃO
                // ===============================
                if (_orcamentoId > 0)
                {
                    orcamento = db.Orcamentos
                        .Include(o => o.Itens)
                        .First(o => o.Id == _orcamentoId);

                    // Remove itens antigos
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
                // ATUALIZA CAMPOS
                // ===============================
                orcamento.ClienteId = cliente.Id;
                orcamento.Veiculo = Veiculo.Trim();
                orcamento.Placa = Placa.Trim();
                orcamento.Data = DateTime.Now;
                orcamento.Total = Itens.Sum(i => i.Quantidade * i.ValorUnitario);

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

                // Se era novo, agora passa a ser edição
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

            // Cliente
            ClienteSelecionado = orcamento.Cliente;
            TextoClienteDigitado = orcamento.Cliente.Nome;
            ClienteEndereco = orcamento.Cliente.Endereco;
            ClienteTelefone = orcamento.Cliente.Telefone;

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



        // ===============================
        // INotifyPropertyChanged
        // ===============================
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
