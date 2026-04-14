using System.Windows;
using Lab4_5.Models;
using Lab4_5.Data;
using System.Linq;
using System.Windows.Input;
using System.IO;

namespace Lab4_5
{
    public partial class MainWindow : Window
    {
        private Cursor _customCursor;
        public MainWindow()
        {
            InitializeComponent();
            LoadCustomCursors();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            using (var context = new AppDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);

                if (user != null && user.Password == password)
                {
                    if (user.IsAdmin)
                    {
                        AdminPanel panel = new AdminPanel(user);
                        panel.Show();
                        this.Close();
                    }
                    else
                    {
                        DashboardWindow dashboard = new DashboardWindow(user);
                        dashboard.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Неверный email или пароль.", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
                    MessageBox.Show($"Файл курсора не найден по пути: {cursorPath}", "Ошибка",
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
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}