// DatingApp.Infrastructure/Repositories/UserRepository.cs
using Dapper;
using DatingApp.Core.Interfaces;
using DatingApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public UserRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            try
            {
                using (var connection = await _connectionFactory.CreateConnectionAsync())
                {
                    var sql = "SELECT * FROM \"Users\" WHERE \"Id\" = @Id";
                    var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
                    if (user != null)
                    {
                        try
                        {
                            user.ProfilePhotoUrls = !String.IsNullOrEmpty(user.ProfilePhotoUrlsJson) ? JsonSerializer.Deserialize<List<string>>(user.ProfilePhotoUrlsJson ?? "[]") : new List<string>();
                            user.PromptAnswers = !String.IsNullOrEmpty(user.PromptAnswersJson) ? JsonSerializer.Deserialize<Dictionary<string, string>>(user.PromptAnswersJson ?? "{}") : new Dictionary<string, string>();
                        }
                        catch (JsonException ex)
                        {
                            // Log error and provide default values
                            Console.WriteLine($"Error deserializing user data for user {id}: {ex.Message}");
                            user.ProfilePhotoUrls = new List<string>();
                            user.PromptAnswers = new Dictionary<string, string>();
                        }
                    }
                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            try
            {
                using (var connection = await _connectionFactory.CreateConnectionAsync())
                {
                    var sql = "SELECT * FROM \"Users\" WHERE \"Username\" = @Username";
                    var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
                    if (user != null)
                    {
                        try
                        {
                            user.ProfilePhotoUrls = !String.IsNullOrEmpty(user.ProfilePhotoUrlsJson) ? JsonSerializer.Deserialize<List<string>>(user.ProfilePhotoUrlsJson ?? "[]") : new List<string>();
                            user.PromptAnswers = !String.IsNullOrEmpty(user.PromptAnswersJson) ? JsonSerializer.Deserialize<Dictionary<string, string>>(user.PromptAnswersJson ?? "{}") : new Dictionary<string, string>();
                        }
                        catch (JsonException ex)
                        {
                            // Log error and provide default values
                            Console.WriteLine($"Error deserializing user data for user {username}: {ex.Message}");
                            user.ProfilePhotoUrls = new List<string>();
                            user.PromptAnswers = new Dictionary<string, string>();
                        }
                    }
                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user {username}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(string interests = null, int excludeUserId = 0, int page = 1, int pageSize = 20)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = "SELECT * FROM \"Users\"";
                var parameters = new DynamicParameters();

                if (excludeUserId > 0)
                {
                    sql += " WHERE \"Id\" != @ExcludeUserId";
                    parameters.Add("ExcludeUserId", excludeUserId);
                }

                if (!string.IsNullOrEmpty(interests))
                {
                    var interestList = interests.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    if (interestList.Any())
                    {
                        sql += (excludeUserId > 0 ? " AND " : " WHERE ") + "\"Interests\" LIKE ANY(@InterestPatterns)";
                        parameters.Add("InterestPatterns", interestList.Select(i => $"%{i}%").ToArray());
                    }
                }

                sql += " ORDER BY \"Id\" LIMIT @PageSize OFFSET @Offset";
                parameters.Add("PageSize", pageSize);
                parameters.Add("Offset", (page - 1) * pageSize);

                var users = await connection.QueryAsync<User>(sql, parameters);

                foreach (var user in users)
                {
                    try
                    {
                        user.ProfilePhotoUrls = !String.IsNullOrEmpty(user.ProfilePhotoUrlsJson) ? JsonSerializer.Deserialize<List<string>>(user.ProfilePhotoUrlsJson ?? "[]") : new List<string>();
                        user.PromptAnswers = !String.IsNullOrEmpty(user.PromptAnswersJson) ? JsonSerializer.Deserialize<Dictionary<string, string>>(user.PromptAnswersJson ?? "{}") : new Dictionary<string, string>();
                    }
                    catch (JsonException ex)
                    {
                        // Log error and provide default values
                        Console.WriteLine($"Error deserializing user data for user {user.Id}: {ex.Message}");
                        user.ProfilePhotoUrls = new List<string>();
                        user.PromptAnswers = new Dictionary<string, string>();
                    }
                }

                return users;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByPreferencesAsync(int currentUserId, string interests = null, int page = 1, int pageSize = 20)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                // First, get the current user's preferences
                var currentUserSql = "SELECT \"Gender\", \"LookingFor\", \"MinAge\", \"MaxAge\", \"Country\", \"City\" FROM \"Users\" WHERE \"Id\" = @CurrentUserId";
                var currentUserPrefs = await connection.QueryFirstOrDefaultAsync<dynamic>(currentUserSql, new { CurrentUserId = currentUserId });

                if (currentUserPrefs == null)
                {
                    return new List<User>();
                }

                // Build the query with preference filters
                var sql = "SELECT * FROM \"Users\" WHERE \"Id\" != @CurrentUserId";
                var parameters = new DynamicParameters();
                parameters.Add("CurrentUserId", currentUserId);

                var conditions = new List<string>();

                // Filter by gender preference
                if (!string.IsNullOrEmpty(currentUserPrefs.LookingFor))
                {
                    if (currentUserPrefs.LookingFor == "Ambos")
                    {
                        conditions.Add("(\"Gender\" = 'Hombre' OR \"Gender\" = 'Mujer')");
                    }
                    else
                    {
                        conditions.Add("\"Gender\" = @LookingFor");
                        parameters.Add("LookingFor", currentUserPrefs.LookingFor == "Hombres" ? "Hombre" : "Mujer");
                    }
                }

                // Filter by age range
                if (currentUserPrefs.MinAge > 0)
                {
                    conditions.Add("EXTRACT(YEAR FROM AGE(\"DateOfBirth\")) >= @MinAge");
                    parameters.Add("MinAge", currentUserPrefs.MinAge);
                }

                if (currentUserPrefs.MaxAge > 0)
                {
                    conditions.Add("EXTRACT(YEAR FROM AGE(\"DateOfBirth\")) <= @MaxAge");
                    parameters.Add("MaxAge", currentUserPrefs.MaxAge);
                }

                // Filter by location (optional - can be made configurable)
                if (!string.IsNullOrEmpty(currentUserPrefs.Country))
                {
                    conditions.Add("\"Country\" = @Country");
                    parameters.Add("Country", currentUserPrefs.Country);
                }

                // Add interests filter if provided
                if (!string.IsNullOrEmpty(interests))
                {
                    var interestList = interests.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    if (interestList.Any())
                    {
                        conditions.Add("\"Interests\" LIKE ANY(@InterestPatterns)");
                        parameters.Add("InterestPatterns", interestList.Select(i => $"%{i}%").ToArray());
                    }
                }

                // Combine all conditions
                if (conditions.Any())
                {
                    sql += " AND " + string.Join(" AND ", conditions);
                }

                // Add pagination
                sql += " ORDER BY \"Id\" LIMIT @PageSize OFFSET @Offset";
                parameters.Add("PageSize", pageSize);
                parameters.Add("Offset", (page - 1) * pageSize);

                var users = await connection.QueryAsync<User>(sql, parameters);

                foreach (var user in users)
                {
                    try
                    {
                        user.ProfilePhotoUrls = !String.IsNullOrEmpty(user.ProfilePhotoUrlsJson) ? JsonSerializer.Deserialize<List<string>>(user.ProfilePhotoUrlsJson ?? "[]") : new List<string>();
                        user.PromptAnswers = !String.IsNullOrEmpty(user.PromptAnswersJson) ? JsonSerializer.Deserialize<Dictionary<string, string>>(user.PromptAnswersJson ?? "{}") : new Dictionary<string, string>();
                    }
                    catch (JsonException ex)
                    {
                        // Log error and provide default values
                        Console.WriteLine($"Error deserializing user data for user {user.Id}: {ex.Message}");
                        user.ProfilePhotoUrls = new List<string>();
                        user.PromptAnswers = new Dictionary<string, string>();
                    }
                }

                return users;
            }
        }

        public async Task<int> AddAsync(User user)
        {
            // Serialize lists to JSON
            user.ProfilePhotoUrlsJson = JsonSerializer.Serialize(user.ProfilePhotoUrls ?? new List<string>());
            user.PromptAnswersJson = JsonSerializer.Serialize(user.PromptAnswers ?? new Dictionary<string, string>());

            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = "INSERT INTO \"Users\" (\"Username\", \"PasswordHash\", \"PasswordSalt\", \"Bio\", \"Interests\", \"ProfilePhotoUrlsJson\", \"PromptAnswersJson\", \"CreatedAt\") VALUES (@Username, @PasswordHash, @PasswordSalt, @Bio, @Interests, @ProfilePhotoUrlsJson, @PromptAnswersJson, @CreatedAt) RETURNING \"Id\"";
                return await connection.QuerySingleAsync<int>(sql, user);
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            // Serialize lists to JSON
            user.ProfilePhotoUrlsJson = JsonSerializer.Serialize(user.ProfilePhotoUrls ?? new List<string>());
            user.PromptAnswersJson = JsonSerializer.Serialize(user.PromptAnswers ?? new Dictionary<string, string>());

            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = "UPDATE \"Users\" SET \"PasswordHash\" = @PasswordHash, \"PasswordSalt\" = @PasswordSalt, \"Bio\" = @Bio, \"Interests\" = @Interests, \"ProfilePhotoUrlsJson\" = @ProfilePhotoUrlsJson, \"PromptAnswersJson\" = @PromptAnswersJson,  \"Gender\" = @Gender, \"LookingFor\" = @LookingFor,   \"MinAge\" = @MinAge,    \"MaxAge\" = @MaxAge,  \"Country\" = @Country,  \"City\" = @City, \"DateOfBirth\" = @DateOfBirth  WHERE \"Id\" = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, user);
                return rowsAffected > 0;
            }
        }
    }
}