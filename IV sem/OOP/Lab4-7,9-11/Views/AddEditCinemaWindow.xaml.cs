using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Lab4_5.Models;
using Lab4_5.Data;

namespace Lab4_5
{
    public partial class AddEditCinemaWindow : Window
    {
        public Cinema Cinema { get; private set; }
        public List<Category> AllowedCategories { get; set; }

        public AddEditCinemaWindow(Cinema cinema = null, List<Category> allowedCategories = null)
        {
            InitializeComponent();

            Cinema = cinema ?? new Cinema();
            if (Cinema.Images == null) Cinema.Images = new List<CinemaImage>();

            using (var context = new AppDbContext())
            {
                AllowedCategories = allowedCategories ?? new List<Category>();
                DataContext = this;
            }
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var filePath in dialog.FileNames)
                {
                    try
                    {
                        var image = new CinemaImage
                        {
                            ImageData = File.ReadAllBytes(filePath),
                            FileName = Path.GetFileName(filePath),
                            ContentType = GetContentType(filePath),
                            CinemaId = Cinema.CinemaId
                        };
                        Cinema.Images.Add(image);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                    }
                }
                ImagesItemsControl.Items.Refresh();
            }
        }

        private string GetContentType(string path)
        {
            return Path.GetExtension(path).ToLower() switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesItemsControl.SelectedItem != null)
            {
                Cinema.Images.Remove((CinemaImage)ImagesItemsControl.SelectedItem);
                ImagesItemsControl.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выберите изображение для удаления",
                               "Предупреждение",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateCinemaData())
            {
                if (Cinema.CategoryId < 1 || Cinema.CategoryId > 5)
                {
                    MessageBox.Show("Выберите категорию из списка (1-5)");
                    return;
                }

                DialogResult = true;
                Close();
            }
        }

        private bool ValidateCinemaData()
        {
            if (string.IsNullOrWhiteSpace(Cinema.Name))
            {
                MessageBox.Show("Введите название кинотеатра");
                return false;
            }

            if (Cinema.CategoryId == 0)
            {
                MessageBox.Show("Выберите категорию");
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}