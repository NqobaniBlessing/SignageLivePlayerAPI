using AutoMapper;
using SignageLivePlayerAPI.Models;
using SignageLivePlayerAPI.Models.DTOs;
using SignageLivePlayerAPI.Services.Interfaces;

namespace SignageLivePlayerAPI.Endpoints
{
    public static class SignageLivePlayerEndpoints
    {
        public static void MapSignageLivePLayerEndpoints(this WebApplication app)
        {
            app.MapPost("api/players", (IPlayerService playerService, IMapper mapper, PlayerCreateDTO playerDTO) =>
            {
                var player = mapper.Map<Player>(playerDTO);
                var result = mapper.Map<PlayerCreateDTO>(playerService.CreatePlayer(player));

                return Results.Created("api/players", result);
            });

            app.MapGet("api/players", (IPlayerService playerService, IMapper mapper) =>
            {
                var result = mapper.Map<List<PlayerDTO>>(playerService.GetAll());

                return Results.Ok(result);
            });

            app.MapGet("api/players/{id}", (IPlayerService playerService, int id, IMapper mapper) =>
            {
                var result = mapper.Map<PlayerDTO>(playerService.Get(id));

                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            });

            app.MapPut("api/players/{id}", (IPlayerService playerService, int id, PlayerUpdateDTO playerDTO,
                IMapper mapper) =>
            {
                var player = mapper.Map<Player>(playerDTO);
                var result = playerService.Update(player, id);

                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            });

            app.MapDelete("api/players/{id}", (IPlayerService playerService, int id) =>
            {
                var player = playerService.Get(id);

                if (player is null)
                    return Results.NotFound();

                playerService.Remove(player, id);

                return Results.NoContent();
            });
        }
    }
}
