using Poputi.DataAccess.Daos;
using System.Threading.Tasks;

namespace Poputi.Logic.Interfaces
{
    public interface IUserService
    {
        Task<User> Get(string login);
        ValueTask PostUserAsync(string name, string familyName, string login, string password, string phoneNumber);
        ValueTask<User> GetUserAsync(string login);
    }
}
