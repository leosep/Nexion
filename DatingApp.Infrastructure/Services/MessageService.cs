// DatingApp.Infrastructure/Services/MessageService.cs
using DatingApp.Core.Interfaces;
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMatchRepository _matchRepository;

        public MessageService(IMessageRepository messageRepository, IMatchRepository matchRepository)
        {
            _messageRepository = messageRepository;
            _matchRepository = matchRepository;
        }

        public async Task<bool> SendMessageAsync(int senderId, int receiverId, string content)
        {
            var match = await _matchRepository.GetMatchAsync(senderId, receiverId);
            if (match == null)
            {
                return false; // No hay match, no se puede enviar el mensaje
            }

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = System.DateTime.UtcNow
            };

            await _messageRepository.AddMessageAsync(message);
            return true;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(int user1Id, int user2Id, int limit = 50, int offset = 0)
        {
            return await _messageRepository.GetMessagesAsync(user1Id, user2Id, limit, offset);
        }
    }
}
