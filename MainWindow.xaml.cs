using System.Windows;
using MecAppIN.ViewModels;

namespace MecAppIN
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
