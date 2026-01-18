using ClosedXML.Excel;
using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public string Veiculo { get; set; }
        public string Placa { get; set; }
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
                {
                    BuscarClientes();
                }
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

            Itens = new ObservableCollection<ItemOrcamento>
            {
                new ItemOrcamento
                {
                    Servico = "Esmerilhar cabeçote",
                    Quantidade = 8,
                    ValorUnitario = 30
                },
                new ItemOrcamento
                {
                    Servico = "Retífica bloco",
                    Quantidade = 1,
                    ValorUnitario = 500
                }
            };
            SalvarOrcamentoCommand = new RelayCommand(SalvarOrcamento);
            GerarExcelCommand = new RelayCommand(GerarExcel);
        }

        // ===============================
        // BUSCAR CLIENTES NO BANCO
        // ===============================
        private void BuscarClientes()
        {
            using var db = new AppDbContext();

            Clientes.Clear();

            if (string.IsNullOrWhiteSpace(TextoClienteDigitado))
                return;

            var termo = TextoClienteDigitado?.Trim();

            if (string.IsNullOrWhiteSpace(termo))
                return;

            var lista = db.Clientes
                .Where(c => EF.Functions.Like(c.Nome, $"%{termo}%"))
                .OrderBy(c => c.Nome)
                .Take(20)
                .ToList();


            foreach (var c in lista)
                Clientes.Add(c);
        }

        // ===============================
        // ATUALIZA DADOS AO SELECIONAR
        // ===============================
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
        // OBTÉM OU CRIA CLIENTE
        // ===============================
        private Clientes ObterOuCriarCliente()
        {
            using var db = new AppDbContext();

            // Cliente já selecionado
            if (ClienteSelecionado != null && ClienteSelecionado.Id > 0)
                return ClienteSelecionado;

            // Verifica se já existe pelo nome
            var existente = db.Clientes
                .FirstOrDefault(c => c.Nome == TextoClienteDigitado);

            if (existente != null)
                return existente;

            // Pergunta se deseja salvar
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
        // GERAR EXCEL
        // ===============================
        private void GerarExcel()
        {
            var cliente = ObterOuCriarCliente();
            if (cliente == null)
                return;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Orçamento");

            // CABEÇALHO
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

            // TABELA
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

            wb.SaveAs("orcamento.xlsx");

            MessageBox.Show("Orçamento gerado com sucesso!",
                            "Sucesso",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void SalvarOrcamento()
        {
            using var db = new AppDbContext();

            // garante cliente
            var cliente = ObterOuCriarCliente();
            if (cliente == null)
                return;

            var orcamento = new Orcamentos
            {
                ClienteId = cliente.Id,
                Veiculo = Veiculo,
                Placa = Placa,
                Data = DateTime.Now,
                Total = Itens.Sum(i => i.Quantidade * i.ValorUnitario),
                Itens = Itens.Select(i => new ItemOrcamento
                {
                    Servico = i.Servico,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario
                }).ToList()
            };

            db.Orcamentos.Add(orcamento);
            db.SaveChanges();

            MessageBox.Show("Orçamento salvo com sucesso!",
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }


        // ===============================
        // INotifyPropertyChanged
        // ===============================
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
