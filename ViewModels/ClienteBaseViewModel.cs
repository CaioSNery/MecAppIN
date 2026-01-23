using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MecAppIN.Data;
using MecAppIN.Models;
using Microsoft.EntityFrameworkCore;

namespace MecAppIN.ViewModels
{
    public abstract class ClienteBaseViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Clientes> Clientes { get; set; }
            = new();

        private Clientes _clienteSelecionado;
        public Clientes ClienteSelecionado
        {
            get => _clienteSelecionado;
            set
            {
                _clienteSelecionado = value;
                OnPropertyChanged();

                if (!_isCarregandoEdicao)
                    AtualizarDadosCliente();
            }
        }

        private string _textoClienteDigitado;
        private bool _isSelecionandoCliente;
        private bool _isCarregandoEdicao; 

        public string TextoClienteDigitado
        {
            get => _textoClienteDigitado;
            set
            {
                if (_textoClienteDigitado == value)
                    return;

                _textoClienteDigitado = value;
                OnPropertyChanged();

                if (!_isSelecionandoCliente && !_isCarregandoEdicao)
                    BuscarClientes();
            }
        }

        public string ClienteEndereco { get; set; }
        public string ClienteTelefone { get; set; }

        // ===============================
        // BUSCA
        // ===============================
        protected void BuscarClientes()
        {
            using var db = new AppDbContext();

            Clientes.Clear();

            if (string.IsNullOrWhiteSpace(TextoClienteDigitado))
                return;

            var termo = TextoClienteDigitado.Trim();

            var lista = db.Clientes
                .Where(c => EF.Functions.Like(c.Nome, $"%{termo}%"))
                .Take(20)
                .ToList();

            foreach (var c in lista)
                Clientes.Add(c);
        }

        // ===============================
        // ATUALIZAR DADOS
        // ===============================
        protected void AtualizarDadosCliente()
        {
            if (ClienteSelecionado == null)
                return;

            _isSelecionandoCliente = true;

            TextoClienteDigitado = ClienteSelecionado.Nome;
            ClienteEndereco = ClienteSelecionado.Endereco;
            ClienteTelefone = ClienteSelecionado.Telefone;

            OnPropertyChanged(nameof(ClienteEndereco));
            OnPropertyChanged(nameof(ClienteTelefone));

            _isSelecionandoCliente = false;
        }

        // ===============================
        // 🔥 MÉTODO PARA EDIÇÃO
        // ===============================
        protected void PreencherClienteEmModoEdicao(Clientes cliente)
        {
            if (cliente == null)
                return;

            _isCarregandoEdicao = true;

            ClienteSelecionado = cliente;
            TextoClienteDigitado = cliente.Nome;
            ClienteEndereco = cliente.Endereco;
            ClienteTelefone = cliente.Telefone;

            OnPropertyChanged(nameof(ClienteEndereco));
            OnPropertyChanged(nameof(ClienteTelefone));

            _isCarregandoEdicao = false;
        }

        // ===============================
        // INotifyPropertyChanged
        // ===============================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
