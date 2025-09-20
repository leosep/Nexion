// DatingApp.Web/Hubs/DatingHub.cs
using DatingApp.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.Web.Hubs
{
    public class DatingHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public DatingHub(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public async Task SendMessage(int receiverId, string content)
        {
            var senderId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // 1. Llama al servicio para enviar el mensaje. El servicio maneja la lógica de guardado y verificación del match.
            var messageSent = await _messageService.SendMessageAsync(senderId, receiverId, content);

            if (messageSent)
            {
                // 2. Si el mensaje se envió correctamente, notifica a ambos clientes.
                // Notifica al emisor (el que está enviando)
                await Clients.User(senderId.ToString()).SendAsync("ReceiveMessage", senderId, content);

                // Notifica al receptor (el que recibe)
                await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, content);
            }
        }
    }
}