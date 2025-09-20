// DatingApp.Core/Interfaces/IAuthService.cs
using DatingApp.Core.Models;
using System.Threading.Tasks;

namespace DatingApp.Core.Interfaces
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(string username, string password);
        Task<User> LoginAsync(string username, string password);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}