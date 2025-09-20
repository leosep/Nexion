// DatingApp.Core/Interfaces/IMessageService.cs
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Core.Interfaces
{
    public interface IMessageService
    {
        Task<bool> SendMessageAsync(int senderId, int receiverId, string content);
        Task<IEnumerable<Message>> GetMessagesAsync(int user1Id, int user2Id, int limit = 50, int offset = 0);
    }
}