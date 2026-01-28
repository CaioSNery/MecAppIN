using System;
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
        // ===============================
        // LISTA DE CLIENTES (SUGESTÕES)
        // ===============================
        public ObservableCollection<Clientes> Clientes { get; set; } = new();

        // ===============================
        // FLAGS DE CONTROLE
        // ===============================
        protected bool _isSelecionandoCliente;
        protected bool _isCarregandoEdicao;

        // ===============================
        // CLIENTE SELECIONADO
        // ===============================
        private Clientes _clienteSelecionado;
        public Clientes ClienteSelecionado
        {
            get => _clienteSelecionado;
            set
            {
                if (_clienteSelecionado == value)
                    return;

                _clienteSelecionado = value;
                OnPropertyChanged();

                if (value == null)
                    return;

                _isSelecionandoCliente = true;

                TextoClienteDigitado = value.Nome;
                ClienteEndereco = value.Endereco;
                ClienteTelefone = value.Telefone;

                OnPropertyChanged(nameof(ClienteEndereco));
                OnPropertyChanged(nameof(ClienteTelefone));

                _isSelecionandoCliente = false;
            }
        }

        // ===============================
        // TEXTO DIGITADO PELO USUÁRIO
        // ===============================
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

                // 🔒 NÃO EXECUTA DURANTE SELEÇÃO OU EDIÇÃO
                if (_isSelecionandoCliente || _isCarregandoEdicao)
                    return;

                // 🔥 SE USUÁRIO DIGITOU OUTRO NOME, LIMPA CLIENTE
                if (ClienteSelecionado != null &&
                    !string.Equals(ClienteSelecionado.Nome, value, StringComparison.OrdinalIgnoreCase))
                {
                    ClienteSelecionado = null;
                    ClienteEndereco = string.Empty;
                    ClienteTelefone = string.Empty;

                    OnPropertyChanged(nameof(ClienteEndereco));
                    OnPropertyChanged(nameof(ClienteTelefone));
                }

                BuscarClientes();
            }
        }

        // ===============================
        // ENDEREÇO
        // ===============================
        private string _clienteEndereco;
        public string ClienteEndereco
        {
            get => _clienteEndereco;
            set
            {
                if (_clienteEndereco == value)
                    return;

                _clienteEndereco = value;
                OnPropertyChanged();
            }
        }

        // ===============================
        // TELEFONE
        // ===============================
        private string _clienteTelefone;
        public string ClienteTelefone
        {
            get => _clienteTelefone;
            set
            {
                if (_clienteTelefone == value)
                    return;

                _clienteTelefone = value;
                OnPropertyChanged();
            }
        }

        // ===============================
        // BUSCAR CLIENTES (AUTOCOMPLETE)
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
        // USAR EM MODO EDIÇÃO (OS / ORÇAMENTO)
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
