// DatingApp.Core/Interfaces/IMatchRepository.cs
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Core.Interfaces
{
    public interface IMatchRepository
    {
        // Like/Interest management
        Task AddLikeAsync(int likerId, int likedId);
        Task<bool> HasLikedAsync(int likerId, int likedId);

        // Match management
        Task AddMatchAsync(int user1Id, int user2Id);
        Task<Match> GetMatchAsync(int user1Id, int user2Id);
        Task<IEnumerable<Match>> GetUserMatchesAsync(int userId);
        Task AddMatchAsync(Match match);
        Task<Match> GetMatchByIdAsync(int matchId);
        Task<Match> GetMatchByUserIdsAsync(int user1Id, int user2Id);

        // Legacy methods - deprecated, use the above methods instead
        [Obsolete("Use AddLikeAsync instead")]
        Task AddInterestAsync(int userId, int targetUserId);
        [Obsolete("Use HasLikedAsync instead")]
        Task<bool> HasInterestAsync(int userId, int targetUserId);
    }
}