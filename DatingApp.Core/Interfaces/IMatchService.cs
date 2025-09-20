// DatingApp.Core/Interfaces/IMatchService.cs
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Core.Interfaces
{
    public interface IMatchService
    {
        Task<IEnumerable<Match>> GetUserMatchesAsync(int userId);
        Task<Match> GetMatchByIdAsync(int matchId);
        Task<Match> GetMatchByUserIdsAsync(int user1Id, int user2Id);
        Task<bool> AddInterestAsync(int likerId, int likedId); // Método añadido
    }
}