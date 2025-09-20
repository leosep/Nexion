// DatingApp.Infrastructure/Services/MatchService.cs
using DatingApp.Core.Interfaces;
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Infrastructure.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;

        public MatchService(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<IEnumerable<Match>> GetUserMatchesAsync(int userId)
        {
            return await _matchRepository.GetUserMatchesAsync(userId);
        }

        public async Task<Match> GetMatchByIdAsync(int matchId)
        {
            return await _matchRepository.GetMatchByIdAsync(matchId);
        }

        public async Task<Match> GetMatchByUserIdsAsync(int user1Id, int user2Id)
        {
            return await _matchRepository.GetMatchByUserIdsAsync(user1Id, user2Id);
        }

        public async Task<bool> AddInterestAsync(int likerId, int likedId)
        {
            // Business rule validation
            if (likerId == likedId)
            {
                throw new ArgumentException("Users cannot like themselves");
            }

            if (likerId <= 0 || likedId <= 0)
            {
                throw new ArgumentException("Invalid user IDs");
            }

            // Check if users exist (this would require IUserRepository injection)
            // For now, we'll assume the validation happens at controller level

            // 1. Check if like already exists to prevent duplicates
            var existingLike = await _matchRepository.HasLikedAsync(likerId, likedId);
            if (existingLike)
            {
                return false; // Like already exists
            }

            // 2. Add the like
            await _matchRepository.AddLikeAsync(likerId, likedId);

            // 3. Check if the other person has also liked this user (reciprocal like)
            var hasReciprocalLike = await _matchRepository.HasLikedAsync(likedId, likerId);

            if (hasReciprocalLike)
            {
                // 4. If there's mutual interest, create a match
                await _matchRepository.AddMatchAsync(likerId, likedId);
                return true; // Match created
            }

            // 5. If no reciprocal like, just return false (like recorded but no match)
            return false;
        }
    }
}