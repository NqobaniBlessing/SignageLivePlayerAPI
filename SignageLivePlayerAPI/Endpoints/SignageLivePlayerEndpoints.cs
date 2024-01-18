using AutoMapper;
using SignageLivePlayerAPI.Models;
using SignageLivePlayerAPI.Models.DTOs;
using SignageLivePlayerAPI.Services.Interfaces;
using System.Security.Cryptography;

namespace SignageLivePlayerAPI.Endpoints
{
    public static class SignageLivePlayerEndpoints
    {
        public static void MapSignageLivePLayerEndpoints(this WebApplication app)
        {
            app.MapPost("api/players", (IPlayerService playerService, IMapper mapper, PlayerCreateDTO playerDTO) =>
            {
                var player = mapper.Map<Player>(playerDTO);
                var result = mapper.Map<PlayerDTO>(playerService.CreatePlayer(player));

                return Results.Created($"api/players/{result.id}", result);
            });

            app.MapGet("api/players", (IPlayerService playerService, IMapper mapper) =>
            {
                var result = mapper.Map<List<PlayerDTO>>(playerService.GetAll());

                return Results.Ok(result);
            });

            app.MapGet("api/players/{id:int}", (IPlayerService playerService, int id, IMapper mapper) =>
            {
                if (id <= 0)
                    return Results.BadRequest();

                var result = mapper.Map<PlayerDTO>(playerService.Get(id));

                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            });

            app.MapPut("api/players/{id:int}", (IPlayerService playerService, int id, PlayerUpdateDTO playerDTO,
                IMapper mapper) =>
            {
                if (id <= 0)
                    return Results.BadRequest();

                var player = mapper.Map<Player>(playerDTO);
                var result = mapper.Map<PlayerUpdateDTO>(playerService.Update(player, id));

                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            });

            app.MapDelete("api/players/{id:int}", (IPlayerService playerService, int id) =>
            {
                if (id <= 0)
                    return Results.BadRequest();

                var player = playerService.Get(id);

                if (player is null)
                    return Results.NotFound();

                playerService.Remove(player, id);

                return Results.NoContent();
            });
        }
    }
}
