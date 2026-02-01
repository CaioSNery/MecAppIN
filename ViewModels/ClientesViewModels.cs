
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MecAppIN.Commands;
using MecAppIN.Data;
using MecAppIN.Models;

namespace MecAppIN.ViewModels
{
    public class ClientesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Clientes> Clientes { get; set; }

        private Clientes _clienteSelecionado;
        public Clientes ClienteSelecionado
        {
            get => _clienteSelecionado;
            set { _clienteSelecionado = value; OnPropertyChanged(); }
        }

        private string _textoBusca;
        public string TextoBusca
        {
            get => _textoBusca;
            set
            {
                _textoBusca = value;
                PaginaAtual = 1;
                OnPropertyChanged();
                CarregarClientes();
            }
        }


        private int _paginaAtual = 1;
        private const int TamanhoPagina = 30;

        public int PaginaAtual
        {
            get => _paginaAtual;
            set
            {
                _paginaAtual = value;
                OnPropertyChanged();
            }
        }

        private int _totalPaginas;
        public int TotalPaginas
        {
            get => _totalPaginas;
            set
            {
                _totalPaginas = value;
                OnPropertyChanged();
            }
        }




        public ICommand NovoCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand ProximaPaginaCommand { get; }
        public ICommand PaginaAnteriorCommand { get; }

        public ClientesViewModel()
        {
            Clientes = new ObservableCollection<Clientes>();
            CarregarClientes();

            ClienteSelecionado = new Clientes();


            SalvarCommand = new RelayCommand(Salvar);
            ExcluirCommand = new RelayCommand(Excluir);
            ProximaPaginaCommand = new RelayCommand(ProximaPagina);
            PaginaAnteriorCommand = new RelayCommand(PaginaAnterior);
        }

        private void ProximaPagina()
        {
            if (PaginaAtual >= TotalPaginas)
                return;

            PaginaAtual++;
            CarregarClientes();
        }


        private void PaginaAnterior()
        {
            if (PaginaAtual <= 1)
                return;

            PaginaAtual--;
            CarregarClientes();
        }



        private void CarregarClientes()
        {
            using var db = new AppDbContext();

            Clientes.Clear();

            var query = db.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(TextoBusca))
            {
                var termo = TextoBusca.ToLower();
                query = query.Where(c => c.Nome.ToLower().Contains(termo));
            }
            
            var totalRegistros = query.Count();
            TotalPaginas = (int)Math.Ceiling(
                totalRegistros / (double)TamanhoPagina
            );

            if (PaginaAtual > TotalPaginas && TotalPaginas > 0)
                PaginaAtual = TotalPaginas;

            var clientesPaginados = query
                .OrderBy(c => c.Nome)
                .Skip((PaginaAtual - 1) * TamanhoPagina)
                .Take(TamanhoPagina)
                .ToList();

            foreach (var c in clientesPaginados)
                Clientes.Add(c);
        }




        private void Salvar()
        {
            if (ClienteSelecionado == null)
                ClienteSelecionado = new Clientes();

            if (string.IsNullOrWhiteSpace(ClienteSelecionado.Nome))
                return;

            using var db = new AppDbContext();

            if (ClienteSelecionado.Id == 0)
                db.Clientes.Add(ClienteSelecionado);
            else
                db.Clientes.Update(ClienteSelecionado);

            db.SaveChanges();

            CarregarClientes();
            ClienteSelecionado = new Clientes(); // limpa formulário
        }

        private void Excluir()
        {
            if (ClienteSelecionado == null || ClienteSelecionado.Id == 0)
                return;

            var resultado = System.Windows.MessageBox.Show(
                $"Deseja realmente excluir o cliente \"{ClienteSelecionado.Nome}\"?",
                "Confirmação",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning
            );

            if (resultado != System.Windows.MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            db.Clientes.Remove(ClienteSelecionado);
            db.SaveChanges();

            CarregarClientes();
            ClienteSelecionado = new Clientes();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}