using AutoMapper;
using SignageLivePlayerAPI.Models;
using SignageLivePlayerAPI.Models.DTOs;
using SignageLivePlayerAPI.Services.Interfaces;
using System;

namespace SignageLivePlayerAPI.Endpoints
{
    public static class SignageLivePlayerEndpoints
    {
        public static void MapSignageLivePLayerEndpoints(this WebApplication app)
        {
            app.MapPost("api/players", (HttpContext context, IPlayerService playerService, IMapper mapper, PlayerCreateDTO playerDTO) =>
            {
                var isAcceptableClaim = VerifyEditingClaims(context);

                if (!isAcceptableClaim)
                    return Results.Forbid();

                var player = mapper.Map<Player>(playerDTO);
                var result = mapper.Map<PlayerDTO>(playerService.CreatePlayer(player));

                return Results.Created($"api/players/{result.id}", result);
            }).RequireAuthorization();

            app.MapGet("api/players", (IPlayerService playerService, IMapper mapper) =>
            {
                var result = mapper.Map<List<PlayerDTO>>(playerService.GetAllPlayers());

                return Results.Ok(result);
            }).RequireAuthorization();

            app.MapGet("api/players/{id:int}", (IPlayerService playerService, int id, IMapper mapper) =>
            {
                if (id <= 0)
                    return Results.BadRequest();

                var result = mapper.Map<PlayerDTO>(playerService.GetPlayer(id));

                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            }).RequireAuthorization();

            app.MapPut("api/players/{id:int}", (HttpContext context, IPlayerService playerService, int id, PlayerUpdateDTO playerDTO,
                IMapper mapper) =>
            {
                var isAcceptableClaim = VerifyEditingClaims(context);

                if (!isAcceptableClaim)
                    return Results.Forbid();

                if (id <= 0)
                    return Results.BadRequest();

                var player = mapper.Map<Player>(playerDTO);
                var result = mapper.Map<PlayerUpdateDTO>(playerService.UpdatePlayer(player, id));

                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            }).RequireAuthorization();

            app.MapDelete("api/players/{id:int}", (HttpContext context, IPlayerService playerService, int id) =>
            {
                var isAcceptableClaim = VerifyEditingClaims(context);

                if (!isAcceptableClaim)
                    return Results.Forbid();

                if (id <= 0)
                    return Results.BadRequest();

                var player = playerService.GetPlayer(id);

                if (player is null)
                    return Results.NotFound();

                playerService.RemovePlayer(player, id);

                return Results.NoContent();
            }).RequireAuthorization();

            bool VerifyEditingClaims(HttpContext context)
            {
                var roleClaim = context.User.Claims.FirstOrDefault(u => u.Type.Equals("role"));
                var adminClaim = context.User.Claims.FirstOrDefault(u => u.Type.Equals("admin"));

                var isAcceptableRoleClaim = (roleClaim is not null && (roleClaim.Value.Equals("Content Manager")
                || roleClaim.Value.Equals("Software Developer")));

                var isAcceptableAdminClaim = (adminClaim is not null && adminClaim.Value.Equals("true"));

                return (isAcceptableRoleClaim || isAcceptableAdminClaim);
            }
        }
    }
}
