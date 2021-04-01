using Poputi.DataAccess.Daos;
using System.Threading.Tasks;

namespace Poputi.Logic.Interfaces
{
    public interface IUserService
    {
        Task<User> Get(string login);
    }
}
