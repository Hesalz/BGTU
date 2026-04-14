using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel.DataAnnotations;
using Lab4_5.Models;
using Lab4_5.Data;
using System.Windows.Data;

namespace Lab4_5
{
    public partial class RegistrationWindow : Window
    {
        private Cursor _customCursor;
        private Brush _originalBorderBrush;

        #region Routed Events
        public static readonly RoutedEvent RegistrationStartedEvent =
            EventManager.RegisterRoutedEvent(
                "RegistrationStarted",
                RoutingStrategy.Direct,
                typeof(RoutedEventHandler),
                typeof(RegistrationWindow));

        public static readonly RoutedEvent RegistrationValidatingEvent =
            EventManager.RegisterRoutedEvent(
                "RegistrationValidating",
                RoutingStrategy.Tunnel,
                typeof(RoutedEventHandler),
                typeof(RegistrationWindow));

        public static readonly RoutedEvent RegistrationCompletedEvent =
            EventManager.RegisterRoutedEvent(
                "RegistrationCompleted",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(RegistrationWindow));
        #endregion

        #region Commands
        public static readonly RoutedUICommand ClearFormCommand =
            new RoutedUICommand(
                "Очистить форму",
                "ClearForm",
                typeof(RegistrationWindow));
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty PhoneNumberProperty =
            DependencyProperty.Register(
                "PhoneNumber",
                typeof(string),
                typeof(RegistrationWindow),
                new FrameworkPropertyMetadata(
                    "+375",
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnPhoneNumberChanged),
                    new CoerceValueCallback(CoercePhoneNumber)),
                    new ValidateValueCallback(ValidatePhoneNumber));

        public string PhoneNumber
        {
            get { return (string)GetValue(PhoneNumberProperty); }
            set { SetValue(PhoneNumberProperty, value); }
        }

        private static void OnPhoneNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as RegistrationWindow;
            window?.UpdateClearButtonState();
        }

        private static bool ValidatePhoneNumber(object value)
        {
            string phone = value as string;

            if (string.IsNullOrEmpty(phone))
                return true;

            if (!phone.All(c => char.IsDigit(c) || c == '+'))
                return false;

            if (phone.Count(c => c == '+') > 1 || (phone.Contains('+') && phone[0] != '+'))
                return false;

            return true;
        }


        private static object CoercePhoneNumber(DependencyObject d, object baseValue)
        {
            string phone = baseValue as string ?? string.Empty;

            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            if (!digitsOnly.StartsWith("375"))
            {
                return "+375";
            }
            
            string afterPrefix = digitsOnly.Substring(3);
            afterPrefix = new string(afterPrefix.Take(9).ToArray());
            if (string.IsNullOrEmpty(afterPrefix))
                return "+375";

            return "+375" + afterPrefix;
        }

        #endregion

        public RegistrationWindow()
        {
            InitializeComponent();
            LoadCustomCursors();
            InitializeCommands();
            AttachEventHandlers();
            _originalBorderBrush = this.BorderBrush;

            var binding = new Binding
            {
                Source = this,
                Path = new PropertyPath("PhoneNumber"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            PhoneTextBox.SetBinding(TextBox.TextProperty, binding);

            LastNameTextBox.TextChanged += TextBox_TextChanged;
            FirstNameTextBox.TextChanged += TextBox_TextChanged;
            MiddleNameTextBox.TextChanged += TextBox_TextChanged;
            EmailTextBox.TextChanged += TextBox_TextChanged;
            PhoneTextBox.PreviewTextInput += PhoneTextBox_PreviewTextInput;
            PhoneTextBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, (s, e) => e.Handled = true));
            PhoneTextBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, (s, e) => e.Handled = true));
            PhoneTextBox.TextChanged += TextBox_TextChanged;

            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;

            UpdateClearButtonState();
            UpdateRegisterButtonState();

        }

        #region Event Handlers
        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
                e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(newText, @"^\+?\d{0,12}$");
            }
        }

        private void PhoneTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste || e.Command == ApplicationCommands.Cut)
            {
                e.Handled = true;
            }
        }
        #endregion

        private void AttachEventHandlers()
        {
            this.AddHandler(RegistrationValidatingEvent, new RoutedEventHandler((s, e) =>
            {
                Debug.WriteLine("[TUNNELING] RegistrationValidatingEvent received at RegistrationWindow");
            }));

            this.AddHandler(RegistrationStartedEvent, new RoutedEventHandler((s, e) =>
            {
                Debug.WriteLine("[DIRECT] RegistrationStartedEvent received at RegistrationWindow");
            }));

            this.AddHandler(RegistrationCompletedEvent, new RoutedEventHandler((s, e) =>
            {
                Debug.WriteLine("[BUBBLING] RegistrationCompletedEvent received at RegistrationWindow");
            }));
        }

        private void MainStackPanel_Validating(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[TUNNELING] RegistrationValidatingEvent received at MainStackPanel");
        }

        private void MainStackPanel_Started(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[DIRECT] RegistrationStartedEvent received at MainStackPanel");
        }

        private void MainStackPanel_Completed(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[BUBBLING] RegistrationCompletedEvent received at MainStackPanel");
        }


        private void InitializeCommands()
        {
            CommandBindings.Add(new CommandBinding(
                ClearFormCommand,
                ClearFormCommand_Executed,
                ClearFormCommand_CanExecute));
        }

        private void LoadCustomCursors()
        {
            try
            {
                string cursorPath = @"D:\БГТУ\IV сем\ООП\Lab4-7,9-10\Cursor\arrow.cur";
                if (File.Exists(cursorPath))
                {
                    _customCursor = new Cursor(cursorPath, true);
                    this.Cursor = _customCursor;
                }
                else
                {
                    MessageBox.Show($"Файл курсора не найден: {cursorPath}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки курсора: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Cursor = Cursors.Arrow;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            PhoneTextBox.RaiseEvent(new RoutedEventArgs(RegistrationValidatingEvent, PhoneTextBox));
            PhoneTextBox.RaiseEvent(new RoutedEventArgs(RegistrationStartedEvent, PhoneTextBox));

            if (PhoneNumber.Length < 13)
            {
                ShowValidationError("Номер телефона должен быть в формате +375XXXXXXXXX (12 цифр после +375)");
                return;
            }

            var newUser = new User
            {
                LastName = LastNameTextBox.Text,
                FirstName = FirstNameTextBox.Text,
                MiddleName = MiddleNameTextBox.Text,
                Email = EmailTextBox.Text,
                PhoneNumber = PhoneNumber,
                Password = PasswordBox.Password,
                IsAdmin = false
            };

            var validationContext = new ValidationContext(newUser);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (!Validator.TryValidateObject(newUser, validationContext, validationResults, true))
            {
                ShowValidationError(string.Join("\n", validationResults.Select(r => r.ErrorMessage)));
                return;
            }

            using (var unitOfWork = new UnitOfWork(new AppDbContext()))
            {
                if (unitOfWork.Users.Find(u => u.Email == newUser.Email).Any())
                {
                    ShowValidationError("Пользователь с таким email уже существует.");
                    return;
                }

                unitOfWork.Users.Add(newUser);
                unitOfWork.Complete();

                ShowSuccessMessage("Регистрация успешно завершена!");
                RaiseEvent(new RoutedEventArgs(RegistrationCompletedEvent, this));

                new MainWindow().Show();
                this.Close();
            }
        }

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
            this.BorderBrush = Brushes.Red;
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (s, e) =>
            {
                this.BorderBrush = _originalBorderBrush;
                timer.Stop();
            };
            timer.Start();
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.BorderBrush = Brushes.Green;
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (s, e) =>
            {
                this.BorderBrush = _originalBorderBrush;
                timer.Stop();
            };
            timer.Start();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateRegisterButtonState();
            UpdateClearButtonState();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateRegisterButtonState();
            UpdateClearButtonState();
        }

        private void UpdateClearButtonState()
        {
            var clearButton = this.FindName("ClearButton") as Button;
            if (clearButton != null)
            {
                clearButton.IsEnabled = IsAnyFieldNotEmpty();
            }
        }

        private bool IsAnyFieldNotEmpty()
        {
            return !string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                   !string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                   !string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ||
                   !string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                   (PhoneNumber != "+375" && !string.IsNullOrWhiteSpace(PhoneNumber)) ||
                   !string.IsNullOrWhiteSpace(PasswordBox.Password);
        }

        private void UpdateRegisterButtonState()
        {
            RegisterButton.IsEnabled = IsAnyFieldNotEmpty();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearFormCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LastNameTextBox.Text = string.Empty;
            FirstNameTextBox.Text = string.Empty;
            MiddleNameTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            PhoneNumber = "+375";
            PasswordBox.Password = string.Empty;
        }


        private void ClearFormCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsAnyFieldNotEmpty();
        }

        private void TestEventsButton_Click(object sender, RoutedEventArgs e)
        {
            AnimateButton((Button)sender);

            var mainStackPanel = this.FindName("MainStackPanel") as StackPanel;
            if (mainStackPanel == null)
            {
                Debug.WriteLine("MainStackPanel not found");
                return;
            }

            mainStackPanel.RemoveHandler(RegistrationValidatingEvent, (RoutedEventHandler)MainStackPanel_Validating);
            mainStackPanel.RemoveHandler(RegistrationStartedEvent, (RoutedEventHandler)MainStackPanel_Started);
            mainStackPanel.RemoveHandler(RegistrationCompletedEvent, (RoutedEventHandler)MainStackPanel_Completed);

            mainStackPanel.AddHandler(RegistrationValidatingEvent, (RoutedEventHandler)MainStackPanel_Validating);
            mainStackPanel.AddHandler(RegistrationStartedEvent, (RoutedEventHandler)MainStackPanel_Started);
            mainStackPanel.AddHandler(RegistrationCompletedEvent, (RoutedEventHandler)MainStackPanel_Completed);

            var tempSource = new TextBox { Visibility = Visibility.Collapsed };
            mainStackPanel.Children.Add(tempSource);

            Debug.WriteLine("--- Raising events ---");
            tempSource.RaiseEvent(new RoutedEventArgs(RegistrationValidatingEvent, tempSource));
            tempSource.RaiseEvent(new RoutedEventArgs(RegistrationStartedEvent, tempSource));
            tempSource.RaiseEvent(new RoutedEventArgs(RegistrationCompletedEvent, tempSource));

            mainStackPanel.Children.Remove(tempSource);

            MessageBox.Show("События вызваны в порядке:\n" +
                          "1. Tunneling (Validating)\n" +
                          "2. Direct (Started)\n" +
                          "3. Bubbling (Completed)\n\n" +
                          "Проверьте Output window (Debug) для логов.",
                          "Тест событий",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }


        private void AnimateButton(Button button)
        {
            var originalBackground = button.Background;
            var originalForeground = button.Foreground;

            ColorAnimation bgAnimation = new ColorAnimation
            {
                To = Colors.LightBlue,
                Duration = TimeSpan.FromSeconds(0.3),
                AutoReverse = true
            };

            ColorAnimation textAnimation = new ColorAnimation
            {
                To = Colors.DarkBlue,
                Duration = TimeSpan.FromSeconds(0.3),
                AutoReverse = true
            };

            SolidColorBrush bgBrush = new SolidColorBrush(Colors.Transparent);
            button.Background = bgBrush;
            bgBrush.BeginAnimation(SolidColorBrush.ColorProperty, bgAnimation);

            SolidColorBrush textBrush = new SolidColorBrush(
                originalForeground is SolidColorBrush scb ? scb.Color : Colors.Black);
            button.Foreground = textBrush;
            textBrush.BeginAnimation(SolidColorBrush.ColorProperty, textAnimation);

            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.6)
            };
            timer.Tick += (s, args) =>
            {
                button.Background = originalBackground;
                button.Foreground = originalForeground;
                timer.Stop();
            };
            timer.Start();
        }

        #region Event Accessors
        public event RoutedEventHandler RegistrationStarted
        {
            add => AddHandler(RegistrationStartedEvent, value);
            remove => RemoveHandler(RegistrationStartedEvent, value);
        }

        public event RoutedEventHandler RegistrationValidating
        {
            add => AddHandler(RegistrationValidatingEvent, value);
            remove => RemoveHandler(RegistrationValidatingEvent, value);
        }

        public event RoutedEventHandler RegistrationCompleted
        {
            add => AddHandler(RegistrationCompletedEvent, value);
            remove => RemoveHandler(RegistrationCompletedEvent, value);
        }
        #endregion
    }
}