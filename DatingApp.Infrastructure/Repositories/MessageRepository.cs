using Dapper;
using DatingApp.Core.Interfaces;
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public MessageRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddMessageAsync(Message message)
        {
            var sql = "INSERT INTO \"Messages\" (\"SenderId\", \"ReceiverId\", \"Content\", \"SentAt\") VALUES (@SenderId, @ReceiverId, @Content, @SentAt)";
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                await connection.ExecuteAsync(sql, message);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(int user1Id, int user2Id, int limit = 50, int offset = 0)
        {
            var sql = "SELECT * FROM \"Messages\" WHERE (\"SenderId\" = @User1Id AND \"ReceiverId\" = @User2Id) OR (\"SenderId\" = @User2Id AND \"ReceiverId\" = @User1Id) ORDER BY \"SentAt\" DESC LIMIT @Limit OFFSET @Offset";
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QueryAsync<Message>(sql, new { User1Id = user1Id, User2Id = user2Id, Limit = limit, Offset = offset });
            }
        }
    }
}