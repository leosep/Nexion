// DatingApp.Infrastructure/Repositories/MatchRepository.cs
using Dapper;
using DatingApp.Core.Interfaces;
using DatingApp.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Infrastructure.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public MatchRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Método para agregar un "me gusta" de una sola vía.
        public async Task AddLikeAsync(int likerId, int likedId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "INSERT INTO \"Likes\" (\"LikerId\", \"LikedId\") VALUES (@LikerId, @LikedId) ON CONFLICT DO NOTHING;";
            await connection.ExecuteAsync(sql, new { LikerId = likerId, LikedId = likedId });
        }

        // Método para verificar si el otro usuario ha dado "me gusta".
        public async Task<bool> HasLikedAsync(int likerId, int likedId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT COUNT(*) FROM \"Likes\" WHERE \"LikerId\" = @LikerId AND \"LikedId\" = @LikedId;";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { LikerId = likerId, LikedId = likedId });
            return count > 0;
        }

        // Método para registrar el match mutuo.
        public async Task AddMatchAsync(int user1Id, int user2Id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var sql = "INSERT INTO \"Matches\" (\"User1Id\", \"User2Id\") VALUES (@User1Id, @User2Id) ON CONFLICT DO NOTHING;";
            await connection.ExecuteAsync(sql, new { User1Id = user1Id, User2Id = user2Id });
        }

        // Legacy methods - deprecated, redirect to new implementation
        [Obsolete("Use AddLikeAsync instead")]
        public async Task AddInterestAsync(int userId, int targetUserId)
        {
            await AddLikeAsync(userId, targetUserId);
        }

        [Obsolete("Use HasLikedAsync instead")]
        public async Task<bool> HasInterestAsync(int userId, int targetUserId)
        {
            return await HasLikedAsync(userId, targetUserId);
        }

        public async Task<Match> GetMatchAsync(int user1Id, int user2Id)
        {
            var sql = "SELECT * FROM \"Matches\" WHERE (\"User1Id\" = @User1Id AND \"User2Id\" = @User2Id) OR (\"User1Id\" = @User2Id AND \"User2Id\" = @User1Id)";
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QuerySingleOrDefaultAsync<Match>(sql, new { User1Id = user1Id, User2Id = user2Id });
            }
        }

        public async Task<IEnumerable<Match>> GetUserMatchesAsync(int userId)
        {
            var sql = @"
                SELECT m.*, u1.*, u2.*
                FROM ""Matches"" m
                JOIN ""Users"" u1 ON m.""User1Id"" = u1.""Id""
                JOIN ""Users"" u2 ON m.""User2Id"" = u2.""Id""
                WHERE m.""User1Id"" = @UserId OR m.""User2Id"" = @UserId
            ";

            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var matches = await connection.QueryAsync<Match, User, User, Match>(
                    sql,
                    (match, user1, user2) =>
                    {
                        match.User1 = user1;
                        match.User2 = user2;
                        return match;
                    },
                    new { UserId = userId },
                    splitOn: "Id,Id"
                );

                return matches;
            }
        }

        public async Task AddMatchAsync(Match match)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = "INSERT INTO \"Matches\" (\"User1Id\", \"User2Id\", \"MatchedAt\") VALUES (@User1Id, @User2Id, @MatchedAt) RETURNING \"Id\"";
                await connection.ExecuteAsync(sql, match);
            }
        }

        public async Task<Match> GetMatchByIdAsync(int matchId)
        {
            var sql = "SELECT * FROM \"Matches\" WHERE \"Id\" = @Id";
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Match>(sql, new { Id = matchId });
            }
        }

        public async Task<Match> GetMatchByUserIdsAsync(int user1Id, int user2Id)
        {
            var sql = "SELECT * FROM \"Matches\" WHERE (\"User1Id\" = @User1Id AND \"User2Id\" = @User2Id) OR (\"User1Id\" = @User2Id AND \"User2Id\" = @User1Id)";
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<Match>(sql, new { User1Id = user1Id, User2Id = user2Id });
            }
        }
    }
}