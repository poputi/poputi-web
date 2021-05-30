using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;
using Poputi.DataAccess.Interfaces;
using Poputi.Logic.Interfaces;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Poputi.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly MainContext _context;

        public UserService(IGenericRepository<User> userRepository, MainContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<User> Get(string login)
        {
            return await _userRepository.Read(u => u.Login == login).FirstOrDefaultAsync();
        }

        public async ValueTask PostUserAsync(string name, string familyName, string login, string password, string phoneNumber)
        {
            var user = new User();
            user.LastName = familyName;
            user.FirstMidName = name;
            user.Login = login;
            user.PhoneNumber = phoneNumber;

            var sha256 = new SHA256Managed();
            user.Password = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async ValueTask<User> GetUserAsync(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Login == login);
        }
    }
}
