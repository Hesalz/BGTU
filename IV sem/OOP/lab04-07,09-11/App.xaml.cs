using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace Lab4_5
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.DispatcherUnhandledException += (s, ex) => {
                MessageBox.Show($"Глобальная ошибка: {ex.Exception.Message}");
                ex.Handled = true;
            };
        }

        public App()
        {
            DispatcherUnhandledException += (sender, ex) =>
            {
                MessageBox.Show($"Ошибка: {ex.Exception.Message}");
                File.WriteAllText("error.log", ex.Exception.ToString());
                ex.Handled = true;
            };
        }
    }

}
