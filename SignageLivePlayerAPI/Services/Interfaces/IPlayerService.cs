using SignageLivePlayerAPI.Models;
using System.Linq.Expressions;

namespace SignageLivePlayerAPI.Services.Interfaces
{
    public interface IPlayerService
    {
        Player UpdatePlayer(Player player, int id);
        Player CreatePlayer(Player player);
        List<Player>? GetAllPlayers();
        Player? GetPlayer(int id);
        void RemovePlayer(Player player, int id);
    }
}
