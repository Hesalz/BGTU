using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace CinemaManagement.Models
{
    public class Film : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _filmID;
        public int FilmID
        {
            get => _filmID;
            set
            {
                if (_filmID != value)
                {
                    _filmID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _director;
        public string Director
        {
            get => _director;
            set
            {
                if (_director != value)
                {
                    _director = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _releaseYear;
        public int ReleaseYear
        {
            get => _releaseYear;
            set
            {
                if (_releaseYear != value)
                {
                    _releaseYear = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _genre;
        public string Genre
        {
            get => _genre;
            set
            {
                if (_genre != value)
                {
                    _genre = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _duration;
        public int Duration
        {
            get => _duration;
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte[] _poster;
        public byte[] Poster
        {
            get => _poster;
            set
            {
                if (_poster != value)
                {
                    _poster = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PosterImage));
                }
            }
        }

        public BitmapImage PosterImage
        {
            get
            {
                if (Poster == null || Poster.Length == 0)
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/post.jpg"));

                var image = new BitmapImage();
                using (var stream = new System.IO.MemoryStream(Poster))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                return image;
            }
        }
    }
}
