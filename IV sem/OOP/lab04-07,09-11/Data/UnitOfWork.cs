using Lab4_5.Models;
using System;

namespace Lab4_5.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;

        public IRepository<User> Users { get; private set; }
        public IRepository<Cinema> Cinemas { get; private set; }
        public IRepository<Category> Categories { get; private set; }
        public IRepository<CinemaImage> CinemaImages { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new Repository<User>(_context);
            Cinemas = new Repository<Cinema>(_context);
            Categories = new Repository<Category>(_context);
            CinemaImages = new Repository<CinemaImage>(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
