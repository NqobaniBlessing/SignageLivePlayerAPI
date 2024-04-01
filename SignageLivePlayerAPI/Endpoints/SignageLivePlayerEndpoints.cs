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
            app.MapPost("api/players", (HttpContext context, IPlayerService playerService, 
                IMapper mapper, PlayerCreateDTO playerDTO) =>
            {
                if (playerDTO == null)
                    return Results.BadRequest();

                var player = mapper.Map<Player>(playerDTO);
                var result = mapper.Map<PlayerDTO>(playerService.CreatePlayer(player));

                return Results.Created($"api/players/{result.id}", result);
            }).RequireAuthorization("ViewAndEdit");

            app.MapGet("api/players", (IPlayerService playerService, IMapper mapper) =>
            {
                var result = mapper.Map<List<PlayerDTO>>(playerService.GetAllPlayers());

                return Results.Ok(result);
            }).RequireAuthorization("ViewOnly");

            app.MapGet("api/players/{id:int}", (IPlayerService playerService, int id, IMapper mapper) =>
            {
                if (id <= 0)
                    return Results.BadRequest();

                var result = mapper.Map<PlayerDTO>(playerService.GetPlayer(id));

                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            }).RequireAuthorization("ViewOnly", "ViewAndEdit");

            app.MapPut("api/players/{id:int}", (HttpContext context, IPlayerService playerService, int id, PlayerUpdateDTO playerDTO,
                IMapper mapper) =>
            {
                if (id <= 0 || playerDTO == null)
                    return Results.BadRequest();

                var player = mapper.Map<Player>(playerDTO);
                var result = mapper.Map<PlayerUpdateDTO>(playerService.UpdatePlayer(player, id));

                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            }).RequireAuthorization("ViewAndEdit");

            app.MapDelete("api/players/{id:int}", (HttpContext context, IPlayerService playerService, int id) =>
            {
                if (id <= 0)
                    return Results.BadRequest();

                var player = playerService.GetPlayer(id);

                if (player is null)
                    return Results.NotFound();

                playerService.RemovePlayer(player, id);

                return Results.NoContent();
            }).RequireAuthorization("ViewAndEdit");
        }
    }
}
