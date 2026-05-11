using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CinemaManagement.Dialogs
{
    public partial class GenreInputDialog : Window
    {
        public GenreInputDialog()
        {
            InitializeComponent();
            DataContext = new GenreInputViewModel(this);
        }

        public string Genre => ((GenreInputViewModel)DataContext).Genre;
    }

    public class GenreInputViewModel : INotifyPropertyChanged
    {
        private string genre;
        private GenreInputDialog window;

        public string Genre
        {
            get => genre;
            set
            {
                genre = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSearch));
            }
        }

        public ICommand SearchCommand { get; }
        public bool CanSearch => !string.IsNullOrWhiteSpace(Genre);

        public GenreInputViewModel(GenreInputDialog dialogWindow)
        {
            window = dialogWindow;
            SearchCommand = new RelayCommand(Search, _ => CanSearch);
        }

        private void Search(object obj)
        {
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
