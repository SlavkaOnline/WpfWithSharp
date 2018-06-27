using System.Windows;

namespace WPFwithFsharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mw = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            mw.Show();
        }
    }
}