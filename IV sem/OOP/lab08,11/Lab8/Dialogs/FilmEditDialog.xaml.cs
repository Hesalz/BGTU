using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CinemaManagement;
using CinemaManagement.Models;
using Microsoft.Win32;

namespace CinemaManagement.Dialogs
{
    public partial class FilmEditDialog : Window
    {
        public FilmEditDialog() : this(new Film()) { }

        public FilmEditDialog(Film film)
        {
            if (film == null)
            {
                throw new ArgumentNullException(nameof(film), "Film cannot be null");
            }

            InitializeComponent();
            DataContext = new FilmEditViewModel(film);
            Title = film.FilmID == 0 ? "Добавить фильм" : "Редактировать фильм";
        }

        public Film Film => ((FilmEditViewModel)DataContext)?.Film;
    }

    public class FilmEditViewModel : BaseViewModel
    {
        public Film Film { get; }
        public string Title { get; }
        public ICommand SelectPosterCommand { get; }
        public ICommand SaveCommand { get; }

        public bool CanSave => Film != null && !string.IsNullOrWhiteSpace(Film.Title);

        public FilmEditViewModel(Film film)
        {
            Film = film ?? throw new ArgumentNullException(nameof(film), "Film cannot be null");
            Title = film.FilmID == 0 ? "Добавить фильм" : "Редактировать фильм";

            SelectPosterCommand = new RelayCommand(SelectPoster);
            SaveCommand = new RelayCommand(Save, _ => CanSave);

            Film.PropertyChanged += Film_PropertyChanged;
        }

        private void Film_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Film.Title))
            {
                OnPropertyChanged(nameof(CanSave));
            }
        }

        private void SelectPoster(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
                Title = "Выберите постер фильма"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    Film.Poster = File.ReadAllBytes(dialog.FileName);
                    OnPropertyChanged(nameof(Film));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось загрузить изображение: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save(object obj)
        {
            if (obj is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
            else
            {
                Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this)
                    ?.Close();
            }
        }
    }
}
