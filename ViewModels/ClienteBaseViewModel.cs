using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
                AtualizarDadosCliente();
            }
        }

        private string _textoClienteDigitado;
        private bool _isSelecionandoCliente;

        public string TextoClienteDigitado
        {
            get => _textoClienteDigitado;
            set
            {
                if (_textoClienteDigitado == value)
                    return;

                _textoClienteDigitado = value;
                OnPropertyChanged();

                if (!_isSelecionandoCliente)
                    BuscarClientes();
            }
        }


        public string ClienteEndereco { get; set; }
        public string ClienteTelefone { get; set; }

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


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

}