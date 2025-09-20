// DatingApp.Core/Interfaces/IUserRepository.cs
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<int> AddAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync(string interests = null, int excludeUserId = 0, int page = 1, int pageSize = 20);
        Task<IEnumerable<User>> GetUsersByPreferencesAsync(int currentUserId, string interests = null, int page = 1, int pageSize = 20);
        Task<bool> UpdateUserAsync(User user);
    }
}