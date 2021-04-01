using Poputi.DataAccess.Daos;
using Poputi.DataAccess.Interfaces;
using Poputi.Logic.Interfaces;
using System.Linq;
using System.Threading.Tasks;


namespace Poputi.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        public UserService(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> Get(string login)
        {
            return await _userRepository.Read(u => u.Login == login).FirstOrDefaultAsync();
        }
    }
}
