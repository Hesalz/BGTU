using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CinemaManagement.Dialogs
{
    public partial class DateInputDialog : Window
    {
        public DateInputDialog()
        {
            InitializeComponent();
            DataContext = new DateInputViewModel(this); 
        }

        public DateTime SelectedDate => ((DateInputViewModel)DataContext)?.SelectedDate ?? DateTime.Today;
    }

    public class DateInputViewModel : INotifyPropertyChanged
    {
        private DateTime selectedDate = DateTime.Today;
        private DateInputDialog window;

        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                selectedDate = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }

        public DateInputViewModel(DateInputDialog dialogWindow)
        {
            window = dialogWindow;
            SearchCommand = new RelayCommand(Search);
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
