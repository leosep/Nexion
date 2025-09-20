// DatingApp.Infrastructure/DataInitializer.cs
using DatingApp.Core.Interfaces;
using DatingApp.Core.Models;
using System.Threading.Tasks;

namespace DatingApp.Infrastructure
{
    public static class DataInitializer
    {
        public static async Task InitializeAsync(
            IAuthService authService,
            IUserRepository userRepository)
        {
            // Verifica si ya hay usuarios en la base de datos
            var existingUser = await userRepository.GetByUsernameAsync("juan123");
            if (existingUser != null)
            {
                return; // La base de datos ya está inicializada
            }

            // Create some example users with more secure passwords
            await authService.RegisterAsync("juan123", "TempPass123!");
            await authService.RegisterAsync("ana456", "TempPass456!");
            await authService.RegisterAsync("carlos789", "TempPass789!");
            await authService.RegisterAsync("sofia001", "TempPass001!");
        }
    }
}