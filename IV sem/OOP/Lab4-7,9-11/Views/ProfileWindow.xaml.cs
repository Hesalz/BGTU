using System;
using System.Windows;
using System.Windows.Controls;
using Lab4_5.Data;
using Lab4_5.Models;
using System.ComponentModel.DataAnnotations;
using System.Windows.Markup;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Data.Entity;      
using System.Data.Entity.Validation;

namespace Lab4_5
{
    public partial class ProfileWindow : Window
    {
        private User _currentUser;

        public ProfileWindow(User user)
        {
            InitializeComponent();
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));

            Loaded += ProfileWindow_Loaded;
        }

        private void ProfileWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserData();
            InitializeLanguage();
            InitializeTheme();

            if (!string.IsNullOrEmpty(_currentUser.Language))
            {
                if (_currentUser.Language == "ru")
                {
                    RussianLanguage.IsChecked = true;
                    EnglishLanguage.IsChecked = false;
                }
                else
                {
                    RussianLanguage.IsChecked = false;
                    EnglishLanguage.IsChecked = true;
                }

                UpdateLanguage(_currentUser.Language);
            }
            else
            {
                RussianLanguage.IsChecked = true;
                _currentUser.Language = "ru";
            }
        }


        private void InitializeTheme()
        {
            if (_currentUser.Theme == "Light")
            {
                LightTheme.IsChecked = true;
                DarkTheme.IsChecked = false;
                ApplyTheme("Light");
            }
            else
            {
                DarkTheme.IsChecked = true;
                LightTheme.IsChecked = false;
                ApplyTheme("Dark");
            }

            DarkTheme.Checked += (s, e) =>
            {
                if (DarkTheme.IsChecked == true)
                    UpdateTheme("Dark");
            };

            LightTheme.Checked += (s, e) =>
            {
                if (LightTheme.IsChecked == true)
                    UpdateTheme("Light");
            };
        }

        private void UpdateTheme(string theme)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Database.ExecuteSqlCommand(
                        "UPDATE Users SET Theme = @p0 WHERE Id = @p1",
                        theme,
                        _currentUser.Id
                    );
                    _currentUser.Theme = theme;
                }

                ApplyTheme(theme);

                if (Owner is AdminPanel adminPanel)
                {
                    adminPanel.ApplyTheme(theme);
                }

                if (Owner is DashboardWindow dashboard)
                {
                    dashboard.ApplyTheme(theme);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка смены темы: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void ApplyTheme(string theme)
        {
            try
            {
                var newThemeUri = new Uri($"Themes/{theme}Theme.xaml", UriKind.Relative);
                var newThemeDict = new ResourceDictionary { Source = newThemeUri };

                var dictionaries = Application.Current.Resources.MergedDictionaries;

                var existingTheme = dictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Themes/"));

                if (existingTheme != null)
                    dictionaries.Remove(existingTheme);

                dictionaries.Add(newThemeDict);

                var localDictionaries = this.Resources.MergedDictionaries;

                var existingLocalTheme = localDictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Themes/"));

                if (existingLocalTheme != null)
                    localDictionaries.Remove(existingLocalTheme);

                localDictionaries.Add(newThemeDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка применения темы: {ex.Message}");
            }
        }



        private void InitializeLanguage()
        {
            if (RussianLanguage == null || EnglishLanguage == null)
                return;

            RussianLanguage.Checked += (s, e) =>
            {
                if (RussianLanguage.IsChecked == true)
                    UpdateLanguage("ru");
            };

            EnglishLanguage.Checked += (s, e) =>
            {
                if (EnglishLanguage.IsChecked == true)
                    UpdateLanguage("en");
            };
        }

        private void UpdateLanguage(string lang)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Database.ExecuteSqlCommand(
                        "UPDATE Users SET Language = @p0 WHERE Id = @p1",
                        lang,
                        _currentUser.Id
                    );
                    _currentUser.Language = lang;
                }

                var uri = new Uri($"Languages/{(lang == "ru" ? "Russian" : "English")}.xaml", UriKind.Relative);
                var dict = new ResourceDictionary { Source = uri };

                this.Dispatcher.Invoke(() =>
                {
                    Resources.MergedDictionaries.Clear();
                    Resources.MergedDictionaries.Add(dict);
                    SafeUpdateElements(dict);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка смены языка: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void SafeUpdateElements(ResourceDictionary dict)
        {
            if (dict.Contains("WindowTitle"))
                Title = dict["WindowTitle"].ToString();

            if (ProfileTitleText != null && dict.Contains("ProfileTitle"))
                ProfileTitleText.Text = dict["ProfileTitle"].ToString();

            if (FullNameLabelText != null && dict.Contains("FullNameLabel"))
                FullNameLabelText.Text = dict["FullNameLabel"].ToString();

            if (PhoneLabelText != null && dict.Contains("PhoneLabel"))
                PhoneLabelText.Text = dict["PhoneLabel"].ToString();

            if (RegistrationDateLabelText != null && dict.Contains("RegistrationDateLabel"))
                RegistrationDateLabelText.Text = dict["RegistrationDateLabel"].ToString();

            if (EmailLabelText != null && dict.Contains("EmailLabel"))
                EmailLabelText.Text = dict["EmailLabel"].ToString();

            if (ChangePasswordTitleText != null && dict.Contains("ChangePasswordTitle"))
                ChangePasswordTitleText.Text = dict["ChangePasswordTitle"].ToString();

            if (CurrentPasswordLabelText != null && dict.Contains("CurrentPasswordLabel"))
                CurrentPasswordLabelText.Text = dict["CurrentPasswordLabel"].ToString();

            if (NewPasswordLabelText != null && dict.Contains("NewPasswordLabel"))
                NewPasswordLabelText.Text = dict["NewPasswordLabel"].ToString();

            if (ConfirmPasswordLabelText != null && dict.Contains("ConfirmPasswordLabel"))
                ConfirmPasswordLabelText.Text = dict["ConfirmPasswordLabel"].ToString();

            if (CancelBtn != null && dict.Contains("CancelButton"))
                CancelBtn.Content = dict["CancelButton"].ToString();

            if (SaveBtn != null && dict.Contains("SaveButton"))
                SaveBtn.Content = dict["SaveButton"].ToString();
        }

        private void LoadUserData()
        {
            string fullName = $"{_currentUser.LastName} {_currentUser.FirstName}";
            if (!string.IsNullOrEmpty(_currentUser.MiddleName))
                fullName += $" {_currentUser.MiddleName}";

            FullNameTextBlock.Text = fullName;
            EmailTextBox.Text = _currentUser.Email;
            PhoneTextBlock.Text = _currentUser.PhoneNumber;
            RegistrationDateTextBlock.Text = _currentUser.CreatedAt.ToString("g");

            OldPasswordBox.Password = "";
            NewPasswordBox.Password = "";
            ConfirmPasswordBox.Password = "";

            if (_currentUser.Language == "en")
            {
                EnglishLanguage.IsChecked = true;
                RussianLanguage.IsChecked = false;
            }
            else
            {
                RussianLanguage.IsChecked = true;
                EnglishLanguage.IsChecked = false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newEmail = EmailTextBox.Text.Trim();
                if (!new EmailAddressAttribute().IsValid(newEmail))
                {
                    MessageBox.Show("Введите корректный email", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!string.IsNullOrEmpty(NewPasswordBox.Password))
                {
                    if (!ValidatePasswordChange()) return;
                    _currentUser.Password = NewPasswordBox.Password;
                }

                _currentUser.Email = newEmail;

                using (var context = new AppDbContext())
                {
                    context.Entry(_currentUser).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }

                MessageBox.Show("Данные успешно сохранены!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidatePasswordChange()
        {
            if (string.IsNullOrEmpty(OldPasswordBox.Password))
            {
                MessageBox.Show("Введите текущий пароль для изменения", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (OldPasswordBox.Password != _currentUser.Password)
            {
                MessageBox.Show("Текущий пароль введен неверно", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (NewPasswordBox.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            string appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            if (!System.IO.File.Exists(appPath))
            {
                MessageBox.Show($"EXE-файл не найден: {appPath}");
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = appPath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(appPath)
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перезапуска: {ex.Message}");
                return;
            }

            Application.Current.Shutdown();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}