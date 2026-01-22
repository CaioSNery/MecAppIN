

using System.Windows.Controls;

namespace MecAppIN.Services
{
    public static class NavegacaoService
    {
        public static ContentControl MainContent { get; set; }

        public static void Navegar(UserControl view)
        {
            MainContent.Content = view;
        }
    }

}