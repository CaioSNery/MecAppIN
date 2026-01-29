using System.Windows;

namespace MecAppIN.Views
{
    public partial class PasswordPromptWindow : Window
    {
        public string SenhaDigitada { get; private set; }

        public PasswordPromptWindow()
        {
            InitializeComponent();
            SenhaBox.Focus();
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            SenhaDigitada = SenhaBox.Password;
            DialogResult = true;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
