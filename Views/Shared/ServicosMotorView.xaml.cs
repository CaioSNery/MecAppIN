using System.Windows.Controls;

namespace MecAppIN.Views.Shared
{
    public partial class ServicosMotorView : UserControl
    {
        public ServicosMotorView()
        {
            InitializeComponent();
        }

        private void ServicosBiela_AddingNewItem(object sender, AddingNewItemEventArgs e) { }
        private void PecasBiela_AddingNewItem(object sender, AddingNewItemEventArgs e) { }

        private void ServicosBloco_AddingNewItem(object sender, AddingNewItemEventArgs e) { }
        private void PecasBloco_AddingNewItem(object sender, AddingNewItemEventArgs e) { }

        private void ServicosCabecote_AddingNewItem(object sender, AddingNewItemEventArgs e) { }
        private void PecasCabeCote_AddingNewItem(object sender, AddingNewItemEventArgs e) { }

        private void ServicosEixo_AddingNewItem(object sender, AddingNewItemEventArgs e) { }
        private void PecasEixo_AddingNewItem(object sender, AddingNewItemEventArgs e) { }

        private void ServicosMotor_AddingNewItem(object sender, AddingNewItemEventArgs e) { }
        private void PecasMotor_AddingNewItem(object sender, AddingNewItemEventArgs e) { }
    }
}
