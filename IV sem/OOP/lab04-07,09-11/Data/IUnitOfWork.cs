using Lab4_5.Models;

namespace Lab4_5.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Cinema> Cinemas { get; }
        IRepository<Category> Categories { get; }
        IRepository<CinemaImage> CinemaImages { get; }

        int Complete();
    }
}
