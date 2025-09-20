// DatingApp.Web/Controllers/ChatController.cs
using DatingApp.Core.Interfaces;
using DatingApp.Infrastructure.Services;
using DatingApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic; // Add this using directive for List<string>

namespace DatingApp.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public ChatController(IMatchRepository matchRepository, IUserService userService, IMessageService messageService)
        {
            _matchRepository = matchRepository;
            _userService = userService;
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var matches = await _matchRepository.GetUserMatchesAsync(userId);

            var matchedUsers = matches?.Select(m => new UserViewModel
            {
                Id = m.User1Id == userId ? m.User2Id : m.User1Id,
                Username = (m.User1Id == userId ? m.User2 : m.User1).Username,
                // Use the null-coalescing operator to ensure ProfilePhotoUrls is never null
                ProfilePhotoUrls = (m.User1Id == userId ? m.User2 : m.User1).ProfilePhotoUrls ?? new List<string>()
            }).ToList() ?? new List<UserViewModel>();

            return View(matchedUsers);
        }

        [HttpGet]
        public async Task<IActionResult> Chat(int otherUserId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Obtener los datos necesarios para el ViewModel
            var otherUser = await _userService.GetUserByIdAsync(otherUserId);
            var match = await _matchRepository.GetMatchAsync(currentUserId, otherUserId);
            var messages = await _messageService.GetMessagesAsync(currentUserId, otherUserId, 50, 0); // Get last 50 messages

            // Crear una instancia del ChatViewModel y llenarla con los datos
            var viewModel = new ChatViewModel
            {
                OtherUser = otherUser,
                Match = match,
                Messages = messages.Reverse() // Since DESC, reverse to ASC for display
            };

            // Pasar el ViewModel a la vista
            return View("Chat", viewModel);
        }
    }
}