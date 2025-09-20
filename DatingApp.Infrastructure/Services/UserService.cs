// DatingApp.Infrastructure/Services/UserService.cs
using DatingApp.Core.Interfaces;
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetUsersAsync(string interests = null, int excludeUserId = 0, int page = 1, int pageSize = 20)
        {
            return await _userRepository.GetAllUsersAsync(interests, excludeUserId, page, pageSize);
        }

        public async Task<IEnumerable<User>> GetUsersByPreferencesAsync(int currentUserId, string interests = null, int page = 1, int pageSize = 20)
        {
            return await _userRepository.GetUsersByPreferencesAsync(currentUserId, interests, page, pageSize);
        }

        public async Task UpdateUserProfileAsync(User user)
        {
            // Business logic validations
            if (user.MinAge >= user.MaxAge)
            {
                throw new ArgumentException("La edad mínima debe ser menor que la edad máxima.");
            }

            if (user.DateOfBirth.HasValue)
            {
                var age = DateTime.UtcNow.Year - user.DateOfBirth.Value.Year;
                if (user.DateOfBirth.Value.Date > DateTime.UtcNow.AddYears(-age)) age--; // Adjust for birthday not passed
                if (age < 18)
                {
                    throw new ArgumentException("Debes tener al menos 18 años.");
                }
            }

            // Validate gender and looking for
            var allowedGenders = new[] { "Hombre", "Mujer", "No Binario" };
            if (!string.IsNullOrEmpty(user.Gender) && !allowedGenders.Contains(user.Gender))
            {
                throw new ArgumentException("Género inválido.");
            }

            var allowedLookingFor = new[] { "Hombres", "Mujeres", "Ambos" };
            if (!string.IsNullOrEmpty(user.LookingFor) && !allowedLookingFor.Contains(user.LookingFor))
            {
                throw new ArgumentException("Preferencia de búsqueda inválida.");
            }

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> AddPhotosAsync(int userId, List<string> photoUrls)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Business logic validation
            if (photoUrls == null || photoUrls.Count == 0)
            {
                return false;
            }

            // Ensure ProfilePhotoUrls is initialized
            if (user.ProfilePhotoUrls == null)
            {
                user.ProfilePhotoUrls = new List<string>();
            }

            // Limit to 6 photos
            if (user.ProfilePhotoUrls.Count >= 6)
            {
                return false; // Or throw exception
            }

            // Add new photos (avoid duplicates)
            foreach (var photoUrl in photoUrls)
            {
                if (!string.IsNullOrWhiteSpace(photoUrl) && !user.ProfilePhotoUrls.Contains(photoUrl) && user.ProfilePhotoUrls.Count < 6)
                {
                    user.ProfilePhotoUrls.Add(photoUrl);
                }
            }

            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> UpdatePromptAnswersAsync(int userId, Dictionary<string, string> promptAnswers)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Business logic validation
            if (promptAnswers == null)
            {
                promptAnswers = new Dictionary<string, string>();
            }

            // Validate prompt answers (ensure they're not too long)
            var validatedAnswers = new Dictionary<string, string>();
            foreach (var kvp in promptAnswers)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Key) && kvp.Value != null && kvp.Value.Length <= 500)
                {
                    validatedAnswers[kvp.Key] = kvp.Value.Trim();
                }
            }

            user.PromptAnswers = validatedAnswers;
            return await _userRepository.UpdateUserAsync(user);
        }
    }
}
