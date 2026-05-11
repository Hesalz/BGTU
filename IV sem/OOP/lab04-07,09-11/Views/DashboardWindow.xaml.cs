using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Lab4_5.Data;
using Lab4_5.Models;
using System.Data.Entity;

namespace Lab4_5
{
    public partial class DashboardWindow : Window
    {
        private IEnumerable<Cinema> _allCinemas;
        private Cursor _customCursor;
        private readonly User _currentUser;
        private ResourceDictionary _currentLanguageDict;

        public DashboardWindow(User user)
        {
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));
            InitializeComponent();
            LoadLanguage(_currentUser.Language ?? "ru");

            ApplyTheme(_currentUser.Theme ?? "Dark");
            LoadCustomCursors();
            CheckDatabaseConnection();
            LoadCinemas();
            InitializeFilters();
        }

        public void ApplyTheme(string theme)
        {
            try
            {
                var newThemeUri = new Uri($"Themes/{theme}Theme.xaml", UriKind.Relative);
                var newThemeDict = new ResourceDictionary { Source = newThemeUri };

                var dictionaries = this.Resources.MergedDictionaries;

                var existingTheme = dictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Themes/"));

                if (existingTheme != null)
                    dictionaries.Remove(existingTheme);

                dictionaries.Add(newThemeDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка применения темы: {ex.Message}");
            }
        }

        private void LoadLanguage(string lang)
        {
            try
            {
                var uri = new Uri($"Languages/{(lang == "ru" ? "Russian" : "English")}.xaml", UriKind.Relative);
                var newDict = new ResourceDictionary { Source = uri };

                Resources.MergedDictionaries.Clear();
                Resources.MergedDictionaries.Add(newDict);
                _currentLanguageDict = newDict;

                Title = newDict.Contains("DashboardWindowTitle")
                    ? newDict["DashboardWindowTitle"].ToString()
                    : "CinemaHub";

                UpdateStaticTexts();
                InitializeFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Language load error: {ex.Message}");
            }
        }

        private void UpdateStaticTexts()
        {
            if (SearchTextBox == null) return;

            if (SearchTextBox.Template?.FindName("placeholderText", SearchTextBox) is TextBlock placeholder)
            {
                placeholder.Text = _currentLanguageDict.Contains("SearchPlaceholder")
                    ? _currentLanguageDict["SearchPlaceholder"].ToString()
                    : "Search...";
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

        private void CheckDatabaseConnection()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    if (!context.Database.Exists())
                    {
                        MessageBox.Show("Нет подключения к базе данных");
                        return;
                    }
                    EnsureCategoriesExist(context);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }

        private void EnsureCategoriesExist(AppDbContext context)
        {
            var requiredCategories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Мультиплекс" },
                new Category { CategoryId = 2, Name = "Арт-хаус" },
                new Category { CategoryId = 3, Name = "IMAX" },
                new Category { CategoryId = 4, Name = "Детский" },
                new Category { CategoryId = 5, Name = "Премиум" }
            };

            foreach (var category in requiredCategories)
            {
                if (!context.Categories.Any(c => c.CategoryId == category.CategoryId))
                {
                    context.Categories.Add(category);
                }
            }
            context.SaveChanges();
        }

        private void LoadCinemas()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    _allCinemas = context.Cinemas
                        .Include(c => c.Category)
                        .Include(c => c.Images)
                        .ToList();

                    CinemaListBox.ItemsSource = _allCinemas;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки кинотеатров: {ex.Message}\n\n{ex.InnerException?.Message}");
            }
        }

        private void InitializeFilters()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var allowedCategories = context.Categories
                        .Where(c => c.CategoryId >= 1 && c.CategoryId <= 5)
                        .OrderBy(c => c.Name)
                        .ToList();

                    var allCategories = new List<Category> {
                new Category {
                    CategoryId = 0,
                    Name = _currentLanguageDict.Contains("AllCategories")
                        ? _currentLanguageDict["AllCategories"].ToString()
                        : "Все категории"
                }
            };
                    allCategories.AddRange(allowedCategories);

                    CategoryFilter.ItemsSource = allCategories;
                    CategoryFilter.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            if (_allCinemas == null) return;

            try
            {
                var filtered = _allCinemas.AsEnumerable();

                string searchText = SearchTextBox.Text?.ToLower() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    filtered = filtered.Where(c =>
                        (c.Name?.ToLower().Contains(searchText) ?? false) ||
                        (c.Address?.ToLower().Contains(searchText) ?? false));
                }

                if (CategoryFilter.SelectedItem is Category selectedCategory && selectedCategory.CategoryId > 0)
                {
                    filtered = filtered.Where(c => c.CategoryId == selectedCategory.CategoryId);
                }

                if (HallCountFilter.SelectedIndex == 1)
                    filtered = filtered.Where(c => c.HallCount >= 1 && c.HallCount <= 3);
                else if (HallCountFilter.SelectedIndex == 2)
                    filtered = filtered.Where(c => c.HallCount >= 4 && c.HallCount <= 7);
                else if (HallCountFilter.SelectedIndex == 3)
                    filtered = filtered.Where(c => c.HallCount >= 8);

                if (CapacityFilter.SelectedIndex == 1)
                    filtered = filtered.Where(c => c.Capacity <= 100);
                else if (CapacityFilter.SelectedIndex == 2)
                    filtered = filtered.Where(c => c.Capacity > 100 && c.Capacity <= 300);
                else if (CapacityFilter.SelectedIndex == 3)
                    filtered = filtered.Where(c => c.Capacity > 300);

                filtered = filtered.Where(c => c.Rating >= RatingFilter.Value);

                CinemaListBox.ItemsSource = filtered.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Filters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void RatingFilter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ApplyFilters();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            CategoryFilter.SelectedIndex = 0;
            HallCountFilter.SelectedIndex = 0;
            CapacityFilter.SelectedIndex = 0;
            RatingFilter.Value = 0;
        }

        private void CinemaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CinemaListBox.SelectedItem is Cinema selectedCinema)
            {
                DetailsPanel.Visibility = Visibility.Visible;
                DetailsPanel.DataContext = selectedCinema;

                using (var context = new AppDbContext())
                {
                    var cinemaWithImages = context.Cinemas
                        .Include(c => c.Images)
                        .FirstOrDefault(c => c.CinemaId == selectedCinema.CinemaId);

                    if (cinemaWithImages != null)
                    {
                        bool hasImages = cinemaWithImages.Images?.Any() ?? false;

                        ImagesItemsControl.ItemsSource = hasImages ? cinemaWithImages.Images : null;
                        ImagesItemsControl.Visibility = hasImages ? Visibility.Visible : Visibility.Collapsed;
                        NoImagesText.Visibility = hasImages ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
            else
            {
                DetailsPanel.Visibility = Visibility.Collapsed;
                DetailsPanel.DataContext = null;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var profileWindow = new ProfileWindow(_currentUser)
                {
                    Owner = this
                };

                profileWindow.Closed += (s, args) =>
                {
                    if (!string.IsNullOrEmpty(_currentUser.Language))
                    {
                        LoadLanguage(_currentUser.Language);
                    }
                };

                profileWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия профиля: {ex.Message}");
            }
        }
    }
}