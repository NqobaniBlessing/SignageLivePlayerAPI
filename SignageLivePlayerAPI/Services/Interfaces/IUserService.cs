using SignageLivePlayerAPI.Models;
using SignageLivePlayerAPI.Models.DTOs;
using System.Linq.Expressions;

namespace SignageLivePlayerAPI.Services.Interfaces
{
    public interface IUserService
    {
        User UpdateUser(User user, Guid id);
        User CreateUser(User user);
        List<User>? GetAllUsers();
        User? GetUser(Guid id);
        void RemoveUser(User user, Guid id);
        bool AuthenticateUser(UserAuthDTO user);
    }
}
