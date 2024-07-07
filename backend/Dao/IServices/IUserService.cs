using backend.Entity;

namespace webapi.Dao.IServices
{
    public interface IUserService
    {
        Task<User> IsValidUserAsync(string username, string password);

        UserRefreshTokens AddUserRefreshTokens(UserRefreshTokens user);

        UserRefreshTokens GetSavedRefreshTokens(string username, string refreshtoken);

        void DeleteUserRefreshTokens(string username, string refreshToken);

        int SaveCommit();
    }
}
