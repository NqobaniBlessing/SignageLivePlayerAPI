using SignageLivePlayerAPI.Models;
using System.Linq.Expressions;

namespace SignageLivePlayerAPI.Services.Interfaces
{
    public interface IPlayerService
    {
        Player Update(Player player, int id);
        Player CreatePlayer(Player player);
        List<Player>? GetAll(Expression<Func<Player, bool>>? filter = null);
        Player? Get(int id);
        void Remove(Player player, int id);
        bool Exists(int id);
    }
}
