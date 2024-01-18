using SignageLivePlayerAPI.Models;
using SignageLivePlayerAPI.Services.Interfaces;

namespace SignageLivePlayerAPI.Endpoints
{
    public static class SignageLivePlayerEndpoints
    {
        public static void MapSignageLivePLayerEndpoints(this WebApplication app)
        {
            app.MapPost("players", (IPlayerService playerService, Player player) =>
            {
                var result = playerService.CreatePlayer(player);

                return result;
            });

            app.MapGet("players", (IPlayerService playerService) =>
            {
                var result = playerService.GetAll();

                return result;
            });

            app.MapGet("players/{id}", (IPlayerService playerService, int id) =>
            {
                var result = playerService.Get(id);

                return result;
            });

            app.MapPut("players/{id}", (IPlayerService playerService, int id, Player player) =>
            {
                var result = playerService.Update(player, id);

                return result;
            });

            app.MapDelete("players/{id}", (IPlayerService playerService, int id) =>
            {
                var player = playerService.Get(id);
                playerService.Remove(player, id);
            });
        }
    }
}
