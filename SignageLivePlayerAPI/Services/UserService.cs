using SignageLivePlayerAPI.Models;
using SignageLivePlayerAPI.Models.DTOs;
using SignageLivePlayerAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SignageLivePlayerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly string filePath = "Data/Users.json";
        private readonly ILogger<UserService> _logger;

        // Values for hashing
        private const int keySize = 64;
        private const int iterations = 350000;
        private HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        public User CreateUser(User user)
        {
            user.Salt = GenerateSalt();
            var hash = HashPasword(user.Password, user.Salt);
            user.Password = hash;

            user.DateCreated = DateTime.UtcNow;
            user.UniqueId = Guid.NewGuid();

            // Add default claims for every created user
            user.Claims.Add("id", user.UniqueId.ToString());
            user.Claims.Add("email", user.UserName);
            user.Claims.Add("jti", Guid.NewGuid().ToString());
            user.Claims.Add("admin", "false");
            user.Claims.Add("role", "user");

            AppendToJson(user, filePath);

            return user;
        }

        public List<User>? GetAllUsers()
        {
            var users = LoadFromJson<List<User>>(filePath);

            return users;
        }

        public User? GetUser(Guid id)
        {
            var users = LoadFromJson<List<User>>(filePath);

            var user = users?.Find(u => u.UniqueId.Equals(id));

            return user;
        }

        public void RemoveUser(User user, Guid id)
        {
            var users = GetAllUsers();

            var userToRemove = users.FirstOrDefault(u => u.UniqueId.Equals(id));
            
            if (userToRemove != null)
            {
                users.Remove(userToRemove);

                SaveToJson(users, filePath);
            }
        }

        public User UpdateUser(User user, Guid id)
        {
            var users = GetAllUsers();

            if (users.Count.Equals(0))
                return null;

            var userToUpdateIndex = users.FindIndex(u => u.UniqueId.Equals(id));

            if (userToUpdateIndex == -1)
            {
                return null;
            }
            else
            {
                user.DateCreated = users[userToUpdateIndex].DateCreated;
                user.DateModified = DateTime.UtcNow;
                users[userToUpdateIndex] = user;

                SaveToJson(users, filePath);
            }

            return user;
        }

        // Utility Functions

        private T? LoadFromJson<T>(string filePath)
        {
            string json = string.Empty;

            try
            {
                json = File.ReadAllText(filePath);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
        }

        private void SaveToJson<T>(T data, string filePath)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private void AppendToJson<T>(T newData, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // If the file exists, append the new data to the existing content
                    string existingJson = File.ReadAllText(filePath);

                    if (existingJson.Equals(string.Empty))
                    {
                        var newDataList = new List<T> { newData };
                        SaveToJson(newDataList, filePath);
                    }
                    else
                    {
                        string newJson = JsonSerializer.Serialize(newData, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        // Add a comma between existing JSON and new JSON
                        string appendedJson = $"{existingJson.TrimEnd(']', '\r', '\n')},\n{newJson}\n]";

                        File.WriteAllText(filePath, appendedJson);
                    }
                }
                else
                {
                    // If the file doesn't exist, create a new file with the new data
                    var newDataList = new List<T> { newData };
                    SaveToJson(newDataList, filePath);
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(keySize);
            
            return salt;
        }

        private string HashPasword(string password, byte[] salt)
        {
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }

        private bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);

            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }

        public bool AuthenticateUser(UserAuthDTO user)
        {
            var users = GetAllUsers();
            var currentUser = users.Find(u => u.UserName.Equals(user.UserName));

            if (currentUser == null)
                return false;

            return VerifyPassword(user.Password, currentUser.Password, currentUser.Salt);
        }
    }
}
