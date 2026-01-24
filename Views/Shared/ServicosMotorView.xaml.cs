using System.Windows.Controls;
using MecAppIN.Enums;

namespace MecAppIN.Views.Shared
{
    public partial class ServicosMotorView : UserControl
    {
        public ServicosMotorView()
        {
            InitializeComponent();
        }

        private void ServicosBiela_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasBiela_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Biela,
                IsPeca = true,
                Quantidade = 1
            };
        }


        private void ServicosBloco_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasBloco_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Bloco,
                IsPeca = true,
                Quantidade = 1
            };
        }


        private void ServicosCabecote_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasCabeCote_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Cabecote,
                IsPeca = true,
                Quantidade = 1
            };
        }


        private void ServicosEixo_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasEixo_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Eixo,
                IsPeca = true,
                Quantidade = 1
            };
        }


        private void ServicosMotor_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                IsPeca = false,
                Quantidade = 1,
                ValorEditavel = true
            };
        }

        private void PecasMotor_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new ItemOrdemServico
            {
                Bloco = EBlocoMotor.Motor,
                IsPeca = true,
                Quantidade = 1
            };
        }

    }
}
