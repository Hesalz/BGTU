

using DAL_Celebrity.Interfaces;
using DAL_Celebrity_MSSQL.Models;

namespace DAL_Celebrity_MSSQL.Interfaces
{
    public interface IRepository: IRepository<Celebrity, Lifeevent> { } 

}
