using SignageLivePlayerAPI.Models;
using SignageLivePlayerAPI.Services.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SignageLivePlayerAPI.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly string filePath = "Data/Players.json";

        public Player CreatePlayer(Player player)
        {
            player.DateCreated = DateTime.UtcNow;
            player.Id = GetNextId();
            player.UniqueId = Guid.NewGuid();
            
            AppendToJson(player, filePath);

            return player;
        }

        public List<Player>? GetAllPlayers()
        {
            var players = LoadFromJson<List<Player>>(filePath);

            return players;
        }

        public Player? GetPlayer(int id)
        {
            var players = LoadFromJson<List<Player>>(filePath);

            var player = players.FirstOrDefault(p => p.Id == id);

            return player;
        }

        public void RemovePlayer(Player player, int id)
        {
            var players = GetAllPlayers();

            var playerToRemove = players.Find(p => p.Id == id);
            
            if (playerToRemove != null)
            {
                players.Remove(playerToRemove);

                SaveToJson(players, filePath);
            }
        }

        public Player UpdatePlayer(Player player, int id)
        {
            var players = GetAllPlayers();

            if (players.Count.Equals(0))
                return null;

            var playerToUpdateIndex = players.FindIndex(p => p.Id == id);

            if (playerToUpdateIndex == -1)
            {
                return null;
            }
            else
            {
                player.DateCreated = players[playerToUpdateIndex].DateCreated;
                player.DateModified = DateTime.UtcNow;
                players[playerToUpdateIndex] = player;

                SaveToJson(players, filePath);
            }

            return player;
        }

        // Utility Functions

        private T? LoadFromJson<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);

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

            File.WriteAllText(filePath, json);
        }

        private void AppendToJson<T>(T newData, string filePath)
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

        private int GetNextId()
        {
            var players = GetAllPlayers();
            var nextId = players.Count + 1;
            
            return nextId;
        }
    }
}
