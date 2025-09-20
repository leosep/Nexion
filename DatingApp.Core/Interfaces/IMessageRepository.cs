// DatingApp.Core/Interfaces/IMessageRepository.cs
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Core.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetMessagesAsync(int user1Id, int user2Id, int limit = 50, int offset = 0);
    }
}
