using AutoMapper;
using SignageLivePlayerAPI.Models;
using SignageLivePlayerAPI.Models.DTOs;
using SignageLivePlayerAPI.Services.Interfaces;

namespace SignageLivePlayerAPI.Endpoints
{
    public static class SignageLiveUserrEndpoints
    {
        public static void MapSignageLiveUserEndpoints(this WebApplication app)
        {
            app.MapPost("api/users", (IUserService userService, IMapper mapper, UserCreateDTO userDTO) =>
            {
                if (userDTO == null)
                    return Results.BadRequest();

                var user = mapper.Map<User>(userDTO);
                var result = mapper.Map<UserDTO>(userService.CreateUser(user));

                return Results.Created($"api/users/{result.UniqueId}", result);
            });

            app.MapGet("api/users", (HttpContext context, IUserService userService, IMapper mapper) =>
            {
                // Only admins can access all users
                var adminClaim = context.User.Claims.FirstOrDefault(u => u.Type.Equals("admin"));

                if (adminClaim is null || adminClaim.Value.Equals("false"))
                    return Results.Forbid();

                var result = mapper.Map<List<UserDTO>>(userService.GetAllUsers());

                return Results.Ok(result);
            }).RequireAuthorization();

            app.MapGet("api/users/me", (HttpContext context, IUserService userService, IMapper mapper) =>
            {
                var idClaim = context.User.Claims.FirstOrDefault(u => u.Type.Equals("id"));

                // If Id Claim of authenticated user is null, something is wrong with the application
                if (idClaim == null)
                    return Results.StatusCode(500);

                Guid.TryParse(idClaim.Value, out var id);
                
                var me = userService.GetUser(id); 
                var result = mapper.Map<UserDTO>(me);

                return Results.Ok(result);
            }).RequireAuthorization();

            app.MapPut("api/users/me", (HttpContext context, IUserService userService, UserUpdateMeDTO userDTO) =>
            {
                if (userDTO == null)
                    return Results.BadRequest();

                var idClaim = context.User.Claims.FirstOrDefault(u => u.Type.Equals("id"));

                // If Id Claim of authenticated user is null, something is wrong with the application
                if (idClaim == null)
                    return Results.StatusCode(500);

                Guid.TryParse(idClaim.Value, out var id);

                var user = userService.GetUser(id);
                user.UserName = userDTO.UserName;
                user.Password = userDTO.Password;
                userService.UpdateUser(user, id);

                return Results.NoContent();
            }).RequireAuthorization();

            app.MapDelete("api/users/me", (HttpContext context, IUserService userService) =>
            {
                var idClaim = context.User.Claims.FirstOrDefault(u => u.Type.Equals("id"));

                // If Id Claim of authenticated user is null, something is wrong with the application
                if (idClaim == null)
                    return Results.StatusCode(500);

                Guid.TryParse(idClaim.Value, out var id);

                var user = userService.GetUser(id);

                if (user is null)
                    return Results.NotFound();

                userService.RemoveUser(user, id);

                return Results.NoContent();
            }).RequireAuthorization();

            app.MapPut("api/users/claims", (HttpContext context, IUserService userService, Guid id, Dictionary<string, string> claims,
                IMapper mapper) =>
            {
                // Only admins can add new claims
                var adminClaim = context.User.Claims.FirstOrDefault(u => u.Type.Equals("admin"));

                if (adminClaim is null || adminClaim.Value.Equals("false"))
                    return Results.Forbid();

                var user = userService.GetUser(id);

                if (user == null)
                    return Results.NotFound();
                
                foreach (var kvp in claims)
                {
                    user.Claims.Add(kvp.Key, kvp.Value);
                }
                userService.UpdateUser(user, id);

                return Results.Ok();

            }).RequireAuthorization();

            app.MapDelete("api/users/{id:guid}", (HttpContext context, IUserService userService, Guid id) =>
            {
                // Only admins can force delete someone else's user by providing the id
                var adminClaim = context.User.Claims.FirstOrDefault(u => u.Type.Equals("admin"));

                if (adminClaim is null || adminClaim.Value.Equals("false"))
                    return Results.Forbid();

                var user = userService.GetUser(id);

                if (user is null)
                    return Results.NotFound();

                userService.RemoveUser(user, id);

                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
