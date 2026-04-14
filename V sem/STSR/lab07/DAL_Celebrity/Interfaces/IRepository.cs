
namespace DAL_Celebrity.Interfaces
{
    public interface IRepository<T1, T2> : IMix<T1, T2>, ICelebrity<T1>, ILifeevent<T2> { }

}
