using MecAppIN.Data;
using QuestPDF.Infrastructure;
using System.Windows;

namespace MecAppIN
{
 public partial class App : Application
{
    public App()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        
    }
}


}
