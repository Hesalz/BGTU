using System.Windows;
using CinemaManagement.ViewModels;

namespace CinemaManagement
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