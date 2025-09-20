// DatingApp.Core/Interfaces/IUserService.cs
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetUsersAsync(string interests = null, int excludeUserId = 0, int page = 1, int pageSize = 20);
        Task<IEnumerable<User>> GetUsersByPreferencesAsync(int currentUserId, string interests = null, int page = 1, int pageSize = 20);
        Task UpdateUserProfileAsync(User user);
        Task<bool> AddPhotosAsync(int userId, List<string> photoUrls);
        Task<bool> UpdatePromptAnswersAsync(int userId, Dictionary<string, string> promptAnswers);
    }
}