// DatingApp.Web/Controllers/MessageController.cs
using DatingApp.Core.Interfaces;
using DatingApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims; // Directiva using añadida
using System.Threading.Tasks;

namespace DatingApp.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMatchService _matchService;
        private readonly IUserService _userService;

        public MessageController(IMessageService messageService, IMatchService matchService, IUserService userService)
        {
            _messageService = messageService;
            _matchService = matchService;
            _userService = userService;
        }

        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetMessages([FromQuery] int otherUserId, [FromQuery] int limit = 50, [FromQuery] int offset = 0)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Método corregido de GetMatchById a GetMatchByUserIdsAsync
            var match = await _matchService.GetMatchByUserIdsAsync(currentUserId, otherUserId);
            if (match == null)
            {
                return Unauthorized("No tienes acceso a esta conversación.");
            }

            // Método corregido de GetMessages a GetMessagesAsync
            var messages = await _messageService.GetMessagesAsync(currentUserId, otherUserId, limit, offset);
            var messageViewModels = messages.Select(m => new MessageViewModel
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Content = m.Content,
                SentAt = m.SentAt
            }).ToList();

            return Ok(messageViewModels);
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageViewModel model)
        {
            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Método corregido de SendMessage a SendMessageAsync
            var success = await _messageService.SendMessageAsync(senderId, model.ReceiverId, model.Content);

            if (success)
            {
                return Ok(new { success = true });
            }

            return BadRequest(new { success = false, message = "No se pudo enviar el mensaje." });
        }
    }
}