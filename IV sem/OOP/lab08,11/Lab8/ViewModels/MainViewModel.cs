using CinemaManagement.Models;
using CinemaManagement.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace CinemaManagement.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string connectionString;
        private ObservableCollection<object> currentItems;
        private object selectedItem;
        private string currentViewType = "Film";
        public bool IsFilmView => CurrentViewType == "Film";
        public bool IsScreeningView => CurrentViewType == "Screening";

        public ObservableCollection<object> CurrentItems
        {
            get => currentItems;
            set => SetProperty(ref currentItems, value);
        }

        public object SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public string CurrentViewType
        {
            get => currentViewType;
            set => SetProperty(ref currentViewType, value);
        }

        public ICommand SwitchToFilmsCommand { get; }
        public ICommand SwitchToScreeningsCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ShowFilmsByGenreDialogCommand { get; }
        public ICommand ShowScreeningsByDateDialogCommand { get; }

        public MainViewModel()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CinemaConnectionString"].ConnectionString;
            EnsureDatabaseExists();

            SwitchToFilmsCommand = new RelayCommand(SwitchToFilms);
            SwitchToScreeningsCommand = new RelayCommand(SwitchToScreenings);
            AddCommand = new RelayCommand(AddItem);
            EditCommand = new RelayCommand(EditItem, CanEditOrDelete);
            DeleteCommand = new RelayCommand(DeleteItem, CanEditOrDelete);
            ShowFilmsByGenreDialogCommand = new RelayCommand(ShowFilmsByGenreDialog);
            ShowScreeningsByDateDialogCommand = new RelayCommand(ShowScreeningsByDateDialog);

            LoadFilms();
        }

        private void EnsureDatabaseExists()
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;

            var masterConnectionString = connectionString.Replace($"Initial Catalog={databaseName}", "Initial Catalog=master");

            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                var checkCommand = new SqlCommand($"SELECT db_id('{databaseName}')", connection);
                var result = checkCommand.ExecuteScalar();

                if (result == DBNull.Value || result == null)
                {
                    var createCommand = new SqlCommand($"CREATE DATABASE [{databaseName}]", connection);
                    createCommand.ExecuteNonQuery();
                }
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sqlFilms = @"
            IF OBJECT_ID('Films', 'U') IS NULL 
            BEGIN
                CREATE TABLE Films (
                    FilmID INT PRIMARY KEY IDENTITY(1,1),
                    Title NVARCHAR(100) NOT NULL,
                    Director NVARCHAR(100),
                    ReleaseYear INT,
                    Genre NVARCHAR(50),
                    Duration INT,
                    Poster VARBINARY(MAX)
                )
            END";
                var command = new SqlCommand(sqlFilms, connection);
                command.ExecuteNonQuery();

                var sqlScreenings = @"
            IF OBJECT_ID('Screenings', 'U') IS NULL 
            BEGIN
                CREATE TABLE Screenings (
                    ScreeningID INT PRIMARY KEY IDENTITY(1,1),
                    FilmID INT NOT NULL,
                    ScreeningDateTime DATETIME NOT NULL,
                    HallNumber INT NOT NULL,
                    TicketPrice DECIMAL(10, 2) NOT NULL,
                    CONSTRAINT FK_Screenings_Films FOREIGN KEY (FilmID) REFERENCES Films(FilmID) ON DELETE CASCADE
                )
            END";
                command.CommandText = sqlScreenings;
                command.ExecuteNonQuery();

                var sqlTrigger = @"
                    IF OBJECT_ID('trg_CheckScreeningDate', 'TR') IS NULL
                    BEGIN
                        EXEC('
                            CREATE TRIGGER trg_CheckScreeningDate
                            ON Screenings
                            AFTER INSERT, UPDATE
                            AS
                            BEGIN
                                IF EXISTS (SELECT 1 FROM inserted WHERE ScreeningDateTime < GETDATE())
                                BEGIN
                                    RAISERROR(''Дата сеанса не может быть в прошлом'', 16, 1);
                                    ROLLBACK TRANSACTION;
                                END
                            END
                        ')
                    END";
                command.CommandText = sqlTrigger;
                command.ExecuteNonQuery();

                command.CommandText = sqlTrigger;
                command.ExecuteNonQuery();

                var sqlSpAddFilm = @"
            IF OBJECT_ID('sp_AddFilm', 'P') IS NULL
            BEGIN
                EXEC('
                CREATE PROCEDURE sp_AddFilm
                    @Title NVARCHAR(100),
                    @Director NVARCHAR(100),
                    @ReleaseYear INT,
                    @Genre NVARCHAR(50),
                    @Duration INT,
                    @Poster VARBINARY(MAX) = NULL
                AS
                BEGIN
                    BEGIN TRANSACTION;
                    BEGIN TRY
                        INSERT INTO Films (Title, Director, ReleaseYear, Genre, Duration, Poster)
                        VALUES (@Title, @Director, @ReleaseYear, @Genre, @Duration, @Poster);
                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION;
                        THROW;
                    END CATCH
                END
                ')
            END";
                command.CommandText = sqlSpAddFilm;
                command.ExecuteNonQuery();

                var sqlSpGetFilmsByGenre = @"
            IF OBJECT_ID('sp_GetFilmsByGenre', 'P') IS NULL
            BEGIN
                EXEC('
                CREATE PROCEDURE sp_GetFilmsByGenre
                    @Genre NVARCHAR(50)
                AS
                BEGIN
                    SELECT * FROM Films WHERE Genre = @Genre ORDER BY Title;
                END
                ')
            END";
                command.CommandText = sqlSpGetFilmsByGenre;
                command.ExecuteNonQuery();

                var sqlSpGetScreeningsByDate = @"
            IF OBJECT_ID('sp_GetScreeningsByDate', 'P') IS NULL
            BEGIN
                EXEC('
                CREATE PROCEDURE sp_GetScreeningsByDate
                    @Date DATE
                AS
                BEGIN
                    SELECT s.ScreeningID, f.Title AS FilmTitle, s.ScreeningDateTime,
                           s.HallNumber, s.TicketPrice
                    FROM Screenings s
                    JOIN Films f ON s.FilmID = f.FilmID
                    WHERE CAST(s.ScreeningDateTime AS DATE) = @Date
                    ORDER BY s.ScreeningDateTime;
                END
                ')
            END";
                command.CommandText = sqlSpGetScreeningsByDate;
                command.ExecuteNonQuery();
            }
        }



        private bool CanEditOrDelete(object obj)
        {
            return SelectedItem != null;
        }

        private void SwitchToFilms(object obj)
        {
            CurrentViewType = "Film";
            LoadFilms();
            OnPropertyChanged(nameof(IsFilmView));
            OnPropertyChanged(nameof(IsScreeningView));
        }

        private void SwitchToScreenings(object obj)
        {
            CurrentViewType = "Screening";
            LoadScreenings();
            OnPropertyChanged(nameof(IsFilmView));
            OnPropertyChanged(nameof(IsScreeningView));
        }

        private void LoadFilms()
        {
            var films = new List<Film>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Films ORDER BY Title", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        films.Add(new Film
                        {
                            FilmID = (int)reader["FilmID"],
                            Title = reader["Title"].ToString(),
                            Director = reader["Director"].ToString(),
                            ReleaseYear = (int)reader["ReleaseYear"],
                            Genre = reader["Genre"].ToString(),
                            Duration = (int)reader["Duration"],
                            Poster = reader["Poster"] as byte[]
                        });
                    }
                }
            }
            CurrentItems = new ObservableCollection<object>(films);
        }

        private void LoadScreenings()
        {
            var screenings = new List<Screening>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT s.ScreeningID, s.FilmID, f.Title AS FilmTitle, " +
                    "s.ScreeningDateTime, s.HallNumber, s.TicketPrice " +
                    "FROM Screenings s JOIN Films f ON s.FilmID = f.FilmID " +
                    "ORDER BY s.ScreeningDateTime", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        screenings.Add(new Screening
                        {
                            ScreeningID = (int)reader["ScreeningID"],
                            FilmID = (int)reader["FilmID"],
                            FilmTitle = reader["FilmTitle"].ToString(),
                            ScreeningDateTime = (DateTime)reader["ScreeningDateTime"],
                            HallNumber = (int)reader["HallNumber"],
                            TicketPrice = (decimal)reader["TicketPrice"]
                        });
                    }
                }
            }
            CurrentItems = new ObservableCollection<object>(screenings);
        }

        private void AddItem(object obj)
        {
            if (CurrentViewType == "Film")
            {
                var dialog = new FilmEditDialog();
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            var command = new SqlCommand("sp_AddFilm", connection);
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@Title", dialog.Film.Title);
                            command.Parameters.AddWithValue("@Director", dialog.Film.Director);
                            command.Parameters.AddWithValue("@ReleaseYear", dialog.Film.ReleaseYear);
                            command.Parameters.AddWithValue("@Genre", dialog.Film.Genre);
                            command.Parameters.AddWithValue("@Duration", dialog.Film.Duration);

                            var posterParam = new SqlParameter("@Poster", SqlDbType.VarBinary, -1);
                            posterParam.Value = dialog.Film.Poster ?? (object)DBNull.Value;
                            command.Parameters.Add(posterParam);


                            command.ExecuteNonQuery();
                        }
                        LoadFilms();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении фильма: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                var films = GetAllFilms();
                var dialog = new ScreeningEditDialog(films, newScreening =>
                {
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            var cmd = new SqlCommand(
                                "INSERT INTO Screenings (FilmID, ScreeningDateTime, HallNumber, TicketPrice) " +
                                "VALUES (@FilmID, @DateTime, @Hall, @Price)", connection);

                            cmd.Parameters.AddWithValue("@FilmID", newScreening.FilmID);
                            cmd.Parameters.AddWithValue("@DateTime", newScreening.ScreeningDateTime);
                            cmd.Parameters.AddWithValue("@Hall", newScreening.HallNumber);
                            cmd.Parameters.AddWithValue("@Price", newScreening.TicketPrice);

                            cmd.ExecuteNonQuery();
                        }
                        LoadScreenings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });

                if (dialog.ShowDialog() == true){}
            }
        }

        private void EditItem(object obj)
        {
            if (CurrentViewType == "Film" && SelectedItem is Film film)
            {
                var dialog = new FilmEditDialog(film);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            using (var transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    var command = new SqlCommand(
                                        "UPDATE Films SET Title = @Title, Director = @Director, " +
                                        "ReleaseYear = @ReleaseYear, Genre = @Genre, Duration = @Duration, " +
                                        "Poster = @Poster WHERE FilmID = @FilmID",
                                        connection, transaction);

                                    command.Parameters.AddWithValue("@FilmID", film.FilmID);
                                    command.Parameters.AddWithValue("@Title", dialog.Film.Title);
                                    command.Parameters.AddWithValue("@Director", dialog.Film.Director);
                                    command.Parameters.AddWithValue("@ReleaseYear", dialog.Film.ReleaseYear);
                                    command.Parameters.AddWithValue("@Genre", dialog.Film.Genre);
                                    command.Parameters.AddWithValue("@Duration", dialog.Film.Duration);
                                    command.Parameters.AddWithValue("@Poster", dialog.Film.Poster ?? (object)DBNull.Value);

                                    command.ExecuteNonQuery();
                                    transaction.Commit();
                                }
                                catch
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }
                        LoadFilms();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении фильма: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (CurrentViewType == "Screening" && SelectedItem is Screening screening)
            {
                var films = GetAllFilms();
                var dialog = new ScreeningEditDialog(films, screening, updatedScreening =>
                {
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            var cmd = new SqlCommand(
                                "UPDATE Screenings SET FilmID = @FilmID, ScreeningDateTime = @DateTime, " +
                                "HallNumber = @Hall, TicketPrice = @Price WHERE ScreeningID = @ID", connection);

                            cmd.Parameters.AddWithValue("@ID", updatedScreening.ScreeningID);
                            cmd.Parameters.AddWithValue("@FilmID", updatedScreening.FilmID);
                            cmd.Parameters.AddWithValue("@DateTime", updatedScreening.ScreeningDateTime);
                            cmd.Parameters.AddWithValue("@Hall", updatedScreening.HallNumber);
                            cmd.Parameters.AddWithValue("@Price", updatedScreening.TicketPrice);

                            cmd.ExecuteNonQuery();
                        }
                        LoadScreenings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
                dialog.ShowDialog();
            }
        }

        private void DeleteItem(object obj)
        {
            if (SelectedItem == null) return;

            var message = CurrentViewType == "Film"
                ? "Вы уверены, что хотите удалить этот фильм? Все связанные сеансы также будут удалены."
                : "Вы уверены, что хотите удалить этот сеанс?";

            if (MessageBox.Show(message, "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new SqlCommand();

                        if (CurrentViewType == "Film")
                        {
                            command.CommandText = "DELETE FROM Films WHERE FilmID = @ID";
                            command.Parameters.AddWithValue("@ID", ((Film)SelectedItem).FilmID);
                        }
                        else
                        {
                            command.CommandText = "DELETE FROM Screenings WHERE ScreeningID = @ID";
                            command.Parameters.AddWithValue("@ID", ((Screening)SelectedItem).ScreeningID);
                        }

                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }

                    if (CurrentViewType == "Film")
                        LoadFilms();
                    else
                        LoadScreenings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private List<Film> GetAllFilms()
        {
            var films = new List<Film>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT FilmID, Title FROM Films ORDER BY Title", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        films.Add(new Film
                        {
                            FilmID = (int)reader["FilmID"],
                            Title = reader["Title"].ToString()
                        });
                    }
                }
            }
            return films;
        }

        private void ShowFilmsByGenreDialog(object obj)
        {
            var dialog = new GenreInputDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var films = new List<Film>();
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new SqlCommand("sp_GetFilmsByGenre", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Genre", dialog.Genre);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                films.Add(new Film
                                {
                                    FilmID = reader["FilmID"] != DBNull.Value ? (int)reader["FilmID"] : 0,
                                    Title = reader["Title"] != DBNull.Value ? reader["Title"].ToString() : string.Empty,
                                    Director = reader["Director"] != DBNull.Value ? reader["Director"].ToString() : string.Empty,
                                    ReleaseYear = reader["ReleaseYear"] != DBNull.Value ? (int)reader["ReleaseYear"] : 0,
                                    Genre = reader["Genre"] != DBNull.Value ? reader["Genre"].ToString() : string.Empty,
                                    Duration = reader["Duration"] != DBNull.Value ? (int)reader["Duration"] : 0,
                                    Poster = reader["Poster"] as byte[]
                                });
                            }
                        }
                    }
                    CurrentItems = new ObservableCollection<object>(films);
                    CurrentViewType = "Film";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void ShowScreeningsByDateDialog(object obj)
        {
            var dialog = new DateInputDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var screenings = new List<Screening>();
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new SqlCommand("sp_GetScreeningsByDate", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Date", dialog.SelectedDate);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                screenings.Add(new Screening
                                {
                                    ScreeningID = reader["ScreeningID"] != DBNull.Value ? (int)reader["ScreeningID"] : 0,
                                    FilmID = reader["FilmID"] != DBNull.Value ? (int)reader["FilmID"] : 0,
                                    FilmTitle = reader["FilmTitle"] != DBNull.Value ? reader["FilmTitle"].ToString() : string.Empty,
                                    ScreeningDateTime = reader["ScreeningDateTime"] != DBNull.Value ? (DateTime)reader["ScreeningDateTime"] : DateTime.MinValue,
                                    HallNumber = reader["HallNumber"] != DBNull.Value ? (int)reader["HallNumber"] : 0,
                                    TicketPrice = reader["TicketPrice"] != DBNull.Value ? (decimal)reader["TicketPrice"] : 0
                                });
                            }
                        }
                    }
                    CurrentItems = new ObservableCollection<object>(screenings);
                    CurrentViewType = "Screening";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}