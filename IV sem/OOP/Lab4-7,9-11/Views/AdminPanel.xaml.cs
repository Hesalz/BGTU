using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lab4_5.Models;
using Lab4_5.Data;
using System.IO;

namespace Lab4_5
{
    public partial class AdminPanel : Window
    {
        private IEnumerable<Cinema> _allCinemas;
        private Cursor _customCursor;
        private readonly User _currentUser;
        private ResourceDictionary _currentLanguageDict;
        private readonly CommandManager _commandManager = new CommandManager();

        public AdminPanel(User user)
        {
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));
            InitializeComponent();
            LoadLanguage(_currentUser.Language ?? "ru");

            ApplyTheme(_currentUser.Theme ?? "Dark");
            LoadCustomCursors();

            try
            {
                using (var context = new AppDbContext())
                {
                    if (!context.Database.Exists())
                    {
                        MessageBox.Show("Нет подключения к базе данных");
                        return;
                    }
                    EnsureCategoriesExist();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }

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
                placeholder.Text = _currentLanguageDict.Contains("SearchPlaceholder") ? _currentLanguageDict["SearchPlaceholder"].ToString() : "Search...";
            }
        }

        private void EnsureCategoriesExist()
        {
            using (var context = new AppDbContext())
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

        public async Task LoadCinemas()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    _allCinemas = await context.Cinemas
                        .Include(c => c.Category)
                        .Include(c => c.Images)
                        .ToListAsync();

                    CinemaListBox.ItemsSource = _allCinemas;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки кинотеатров: {ex.Message}\n\n{ex.InnerException?.Message}");
            }
        }

        private async Task InitializeFilters()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var allowedCategories = await context.Categories
                        .Where(c => c.CategoryId >= 1 && c.CategoryId <= 5)
                        .OrderBy(c => c.Name)
                        .ToListAsync();

                    var allCategories = new List<Category> {
                        new Category {
                            CategoryId = 0,
                            Name = _currentLanguageDict.Contains("AllCategories") ? _currentLanguageDict["AllCategories"].ToString() : "Все категории"
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

        private async void AddCinema_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext())
            {
                var allowedCategories = await context.Categories
                    .Where(c => c.CategoryId >= 1 && c.CategoryId <= 5)
                    .ToListAsync();

                var window = new AddEditCinemaWindow(null, allowedCategories);
                if (window.ShowDialog() == true)
                {
                    if (window.Cinema.CategoryId < 1 || window.Cinema.CategoryId > 5)
                    {
                        MessageBox.Show("Пожалуйста, выберите допустимую категорию (1-5)");
                        return;
                    }

                    try
                    {
                        if (window.Cinema.Images == null)
                        {
                            window.Cinema.Images = new List<CinemaImage>();
                        }

                        var command = new AddCinemaCommand(window.Cinema, this);
                        await _commandManager.ExecuteCommandAsync(command);
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                        var fullErrorMessage = string.Join("; ", errorMessages);
                        MessageBox.Show($"Ошибка валидации: {fullErrorMessage}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                    }
                }
            }
        }

        private async void EditCinema_Click(object sender, RoutedEventArgs e)
        {
            if (CinemaListBox.SelectedItem is Cinema selectedCinema)
            {
                using (var context = new AppDbContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = true;

                    try
                    {
                        var cinemaToEdit = await context.Cinemas
                            .Include(c => c.Images)
                            .Include(c => c.Category)
                            .FirstOrDefaultAsync(c => c.CinemaId == selectedCinema.CinemaId);

                        if (cinemaToEdit != null)
                        {
                            var allowedCategories = await context.Categories
                                .Where(c => c.CategoryId >= 1 && c.CategoryId <= 5)
                                .ToListAsync();

                            var originalImages = cinemaToEdit.Images.ToList();
                            var tempImages = originalImages.Select(i => new CinemaImage
                            {
                                ImageId = i.ImageId,
                                ImageData = i.ImageData,
                                FileName = i.FileName,
                                ContentType = i.ContentType,
                                CinemaId = i.CinemaId
                            }).ToList();

                            var editWindow = new AddEditCinemaWindow(new Cinema
                            {
                                CinemaId = cinemaToEdit.CinemaId,
                                Name = cinemaToEdit.Name,
                                Address = cinemaToEdit.Address,
                                HallCount = cinemaToEdit.HallCount,
                                Capacity = cinemaToEdit.Capacity,
                                Rating = cinemaToEdit.Rating,
                                CategoryId = cinemaToEdit.CategoryId,
                                Images = tempImages
                            }, allowedCategories);

                            if (editWindow.ShowDialog() == true)
                            {
                                var originalCinema = new Cinema
                                {
                                    CinemaId = cinemaToEdit.CinemaId,
                                    Name = cinemaToEdit.Name,
                                    Address = cinemaToEdit.Address,
                                    HallCount = cinemaToEdit.HallCount,
                                    Capacity = cinemaToEdit.Capacity,
                                    Rating = cinemaToEdit.Rating,
                                    CategoryId = cinemaToEdit.CategoryId
                                };

                                var command = new EditCinemaCommand(
                                    originalCinema,
                                    editWindow.Cinema,
                                    originalImages,
                                    editWindow.Cinema.Images?.ToList() ?? new List<CinemaImage>(),
                                    this);

                                await _commandManager.ExecuteCommandAsync(command);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorDetails = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        MessageBox.Show($"Ошибка сохранения: {errorDetails}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        context.Configuration.AutoDetectChangesEnabled = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите кинотеатр для редактирования.");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void Filters_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void RatingFilter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => ApplyFilters();
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
                        bool hasImages = cinemaWithImages.Images.Any();

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
        private async void DeleteCinema_Click(object sender, RoutedEventArgs e)
        {
            if (CinemaListBox.SelectedItem is Cinema selectedCinema)
            {
                var result = MessageBox.Show($"Удалить кинотеатр '{selectedCinema.Name}'?",
                    "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new AppDbContext())
                    {
                        var cinemaToDelete = await context.Cinemas
                            .Include(c => c.Images)
                            .FirstOrDefaultAsync(c => c.CinemaId == selectedCinema.CinemaId);

                        if (cinemaToDelete != null)
                        {
                            var imagesCopy = cinemaToDelete.Images.Select(i => new CinemaImage
                            {
                                ImageId = i.ImageId,
                                ImageData = i.ImageData,
                                FileName = i.FileName,
                                ContentType = i.ContentType,
                                CinemaId = i.CinemaId
                            }).ToList();

                            var command = new DeleteCinemaCommand(
                                new Cinema
                                {
                                    CinemaId = cinemaToDelete.CinemaId,
                                    Name = cinemaToDelete.Name,
                                    Address = cinemaToDelete.Address,
                                    HallCount = cinemaToDelete.HallCount,
                                    Capacity = cinemaToDelete.Capacity,
                                    Rating = cinemaToDelete.Rating,
                                    CategoryId = cinemaToDelete.CategoryId
                                },
                                imagesCopy,
                                this);

                            await _commandManager.ExecuteCommandAsync(command);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите кинотеатр для удаления");
            }
        }

        private async void RefreshList_Click(object sender, RoutedEventArgs e) => await LoadCinemas();

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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Undo_Click(object sender, RoutedEventArgs e)
        {
            await _commandManager.UndoAsync();
        }

        private async void Redo_Click(object sender, RoutedEventArgs e)
        {
            await _commandManager.RedoAsync();
        }
    }

    public interface ICustomCommand
    {
        Task ExecuteAsync();
        Task UndoAsync();
    }

    public class CommandManager
    {
        private readonly Stack<ICustomCommand> _undoStack = new Stack<ICustomCommand>();
        private readonly Stack<ICustomCommand> _redoStack = new Stack<ICustomCommand>();

        public async Task ExecuteCommandAsync(ICustomCommand command)
        {
            await command.ExecuteAsync();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public async Task UndoAsync()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                await command.UndoAsync();
                _redoStack.Push(command);
            }
        }

        public async Task RedoAsync()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                await command.ExecuteAsync();
                _undoStack.Push(command);
            }
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
    }

    public class AddCinemaCommand : ICustomCommand
    {
        private readonly Cinema _cinema;
        private readonly AdminPanel _adminPanel;

        public AddCinemaCommand(Cinema cinema, AdminPanel adminPanel)
        {
            _cinema = cinema;
            _adminPanel = adminPanel;
        }

        public async Task ExecuteAsync()
        {
            using (var context = new AppDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Cinemas.Add(_cinema);
                        await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            await _adminPanel.LoadCinemas();
        }

        public async Task UndoAsync()
        {
            using (var context = new AppDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var cinemaToDelete = await context.Cinemas
                            .Include(c => c.Images)
                            .FirstOrDefaultAsync(c => c.CinemaId == _cinema.CinemaId);

                        if (cinemaToDelete != null)
                        {
                            context.CinemaImages.RemoveRange(cinemaToDelete.Images);
                            context.Cinemas.Remove(cinemaToDelete);
                            await context.SaveChangesAsync();
                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            await _adminPanel.LoadCinemas();
        }
    }

    public class EditCinemaCommand : ICustomCommand
    {
        private readonly Cinema _originalCinema;
        private readonly Cinema _modifiedCinema;
        private readonly List<CinemaImage> _originalImages;
        private readonly List<CinemaImage> _modifiedImages;
        private readonly AdminPanel _adminPanel;

        public EditCinemaCommand(Cinema originalCinema, Cinema modifiedCinema,
                               List<CinemaImage> originalImages, List<CinemaImage> modifiedImages,
                               AdminPanel adminPanel)
        {
            _originalCinema = originalCinema;
            _modifiedCinema = modifiedCinema;
            _originalImages = originalImages;
            _modifiedImages = modifiedImages;
            _adminPanel = adminPanel;
        }

        public async Task ExecuteAsync()
        {
            await ApplyChangesAsync(_modifiedCinema, _modifiedImages);
        }

        public async Task UndoAsync()
        {
            await ApplyChangesAsync(_originalCinema, _originalImages);
        }

        private async Task ApplyChangesAsync(Cinema cinema, List<CinemaImage> images)
        {
            using (var context = new AppDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var cinemaToUpdate = await context.Cinemas
                            .Include(c => c.Images)
                            .FirstOrDefaultAsync(c => c.CinemaId == cinema.CinemaId);

                        if (cinemaToUpdate != null)
                        {
                            cinemaToUpdate.Name = cinema.Name;
                            cinemaToUpdate.Address = cinema.Address;
                            cinemaToUpdate.HallCount = cinema.HallCount;
                            cinemaToUpdate.Capacity = cinema.Capacity;
                            cinemaToUpdate.Rating = cinema.Rating;
                            cinemaToUpdate.CategoryId = cinema.CategoryId;

                            var existingImages = cinemaToUpdate.Images.ToList();

                            foreach (var existingImage in existingImages)
                            {
                                if (!images.Any(i => i.ImageId == existingImage.ImageId && i.ImageId != 0))
                                {
                                    context.CinemaImages.Remove(existingImage);
                                }
                            }

                            foreach (var image in images)
                            {
                                if (image.ImageId == 0)
                                {
                                    context.CinemaImages.Add(new CinemaImage
                                    {
                                        ImageData = image.ImageData,
                                        FileName = image.FileName,
                                        ContentType = image.ContentType,
                                        CinemaId = cinemaToUpdate.CinemaId
                                    });
                                }
                                else
                                {
                                    var existingImage = existingImages.FirstOrDefault(i => i.ImageId == image.ImageId);
                                    if (existingImage != null)
                                    {
                                        existingImage.ImageData = image.ImageData;
                                        existingImage.FileName = image.FileName;
                                        existingImage.ContentType = image.ContentType;
                                    }
                                }
                            }

                            await context.SaveChangesAsync();
                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            await _adminPanel.LoadCinemas();
        }
    }

    public class DeleteCinemaCommand : ICustomCommand
    {
        private readonly Cinema _cinema;
        private readonly List<CinemaImage> _images;
        private readonly AdminPanel _adminPanel;

        public DeleteCinemaCommand(Cinema cinema, List<CinemaImage> images, AdminPanel adminPanel)
        {
            _cinema = cinema;
            _images = images;
            _adminPanel = adminPanel;
        }

        public async Task ExecuteAsync()
        {
            using (var context = new AppDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var cinemaToDelete = await context.Cinemas
                            .Include(c => c.Images)
                            .FirstOrDefaultAsync(c => c.CinemaId == _cinema.CinemaId);

                        if (cinemaToDelete != null)
                        {
                            context.CinemaImages.RemoveRange(cinemaToDelete.Images);
                            context.Cinemas.Remove(cinemaToDelete);
                            await context.SaveChangesAsync();
                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            await _adminPanel.LoadCinemas();
        }

        public async Task UndoAsync()
        {
            using (var context = new AppDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Cinemas.Add(_cinema);
                        if (_images != null && _images.Any())
                        {
                            context.CinemaImages.AddRange(_images);
                        }
                        await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            await _adminPanel.LoadCinemas();
        }
    }
}
