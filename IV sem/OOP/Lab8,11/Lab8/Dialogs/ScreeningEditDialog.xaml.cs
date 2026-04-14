using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CinemaManagement.Models;

namespace CinemaManagement.Dialogs
{
    public partial class ScreeningEditDialog : Window
    {
        public ScreeningEditDialog(List<Film> films, Action<Screening> saveAction)
            : this(films, new Screening(), saveAction) { }

        public ScreeningEditDialog(List<Film> films, Screening screening, Action<Screening> saveAction)
        {
            InitializeComponent();

            if (films == null) throw new ArgumentNullException(nameof(films));
            if (screening == null) throw new ArgumentNullException(nameof(screening));
            if (saveAction == null) throw new ArgumentNullException(nameof(saveAction));

            DataContext = new ScreeningEditViewModel(films, screening, saveAction);
            Title = screening.ScreeningID == 0 ? "Добавить сеанс" : "Редактировать сеанс";
        }

        public Screening Screening => ((ScreeningEditViewModel)DataContext).Screening;
    }

    public class ScreeningEditViewModel : BaseViewModel
    {
        private readonly Action<Screening> _saveAction;
        private Film _selectedFilm;
        private DateTime _screeningDate;
        private int _selectedHour;
        private int _selectedMinute;
        private int _hallNumber;
        private decimal _ticketPrice;
        private bool _canSave;

        public List<Film> Films { get; }
        public Screening Screening { get; }
        public ICommand SaveCommand { get; }
        public List<int> Hours { get; } = Enumerable.Range(8, 14).ToList();
        public List<int> Minutes { get; } = new List<int> { 0, 15, 30, 45 };

        public bool CanSave
        {
            get => _canSave;
            private set => SetProperty(ref _canSave, value);
        }

        public ScreeningEditViewModel(List<Film> films, Screening screening, Action<Screening> saveAction)
        {
            Films = films ?? throw new ArgumentNullException(nameof(films));
            Screening = screening ?? throw new ArgumentNullException(nameof(screening));
            _saveAction = saveAction ?? throw new ArgumentNullException(nameof(saveAction));

            _selectedFilm = Films.FirstOrDefault(f => f.FilmID == screening.FilmID);
            _screeningDate = screening.ScreeningDateTime.Date;
            _selectedHour = screening.ScreeningDateTime.Hour;
            _selectedMinute = screening.ScreeningDateTime.Minute;
            _hallNumber = screening.HallNumber;
            _ticketPrice = screening.TicketPrice;

            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave);

            if (screening.ScreeningID == 0)
            {
                SetDefaultScreeningTime();
            }

            UpdateCanSave();
        }

        private void SetDefaultScreeningTime()
        {
            var now = DateTime.Now;
            if (now.Hour < 8)
            {
                ScreeningDate = now.Date;
                SelectedHour = 8;
                SelectedMinute = 0;
            }
            else if (now.Hour >= 21)
            {
                ScreeningDate = now.Date.AddDays(1);
                SelectedHour = 8;
                SelectedMinute = 0;
            }
            else
            {
                ScreeningDate = now.Date;
                SelectedHour = now.Hour;
                SelectedMinute = (now.Minute / 15) * 15;
                if (SelectedMinute < now.Minute) SelectedMinute += 15;
                if (SelectedMinute >= 60)
                {
                    SelectedMinute = 0;
                    SelectedHour++;
                    if (SelectedHour >= 24)
                    {
                        SelectedHour = 0;
                        ScreeningDate = ScreeningDate.AddDays(1);
                    }
                }
            }
        }

        public Film SelectedFilm
        {
            get => _selectedFilm;
            set
            {
                if (SetProperty(ref _selectedFilm, value))
                {
                    Screening.FilmID = value?.FilmID ?? 0;
                    UpdateScreeningDateTime();
                    UpdateCanSave();
                }
            }
        }

        public DateTime ScreeningDate
        {
            get => _screeningDate;
            set
            {
                if (SetProperty(ref _screeningDate, value))
                {
                    UpdateScreeningDateTime();
                    UpdateCanSave();
                }
            }
        }

        public int SelectedHour
        {
            get => _selectedHour;
            set
            {
                if (SetProperty(ref _selectedHour, value))
                {
                    UpdateScreeningDateTime();
                    UpdateCanSave();
                }
            }
        }

        public int SelectedMinute
        {
            get => _selectedMinute;
            set
            {
                if (SetProperty(ref _selectedMinute, value))
                {
                    UpdateScreeningDateTime();
                    UpdateCanSave();
                }
            }
        }

        public int HallNumber
        {
            get => _hallNumber;
            set
            {
                if (SetProperty(ref _hallNumber, value))
                {
                    Screening.HallNumber = value;
                    UpdateCanSave();
                }
            }
        }

        public decimal TicketPrice
        {
            get => _ticketPrice;
            set
            {
                if (SetProperty(ref _ticketPrice, value))
                {
                    Screening.TicketPrice = value;
                    UpdateCanSave();
                }
            }
        }

        private void UpdateScreeningDateTime()
        {
            Screening.ScreeningDateTime = _screeningDate.Date + new TimeSpan(_selectedHour, _selectedMinute, 0);
            OnPropertyChanged(nameof(Screening));
        }

        private void UpdateCanSave()
        {
            CanSave = _selectedFilm != null &&
                  Screening.ScreeningDateTime > DateTime.Now.AddMinutes(-1) &&
                 _hallNumber > 0 &&
                 _ticketPrice > 0;
        }

        private void Save()
        {
            try
            {
                if (!CanSave)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля:\n\n" +
                                   "• Выберите фильм\n" +
                                   "• Укажите будущую дату и время\n" +
                                   "• Введите номер зала (больше 0)\n" +
                                   "• Укажите цену билета (больше 0)",
                                  "Не все поля заполнены",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                _saveAction?.Invoke(Screening);

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.DialogResult = true;
                        window.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении сеанса:\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}