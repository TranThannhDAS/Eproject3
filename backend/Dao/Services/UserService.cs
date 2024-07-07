using backend.Entity;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using webapi.Dao.IServices;
using webapi.Data;

namespace webapi.Dao.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _db;

        public UserService(DataContext _db)
        {
            this._db = _db;
        }

        public UserRefreshTokens AddUserRefreshTokens(UserRefreshTokens user)
        {
            _db.UserRefreshTokens.Add(user);
            return user;
        }

        public void DeleteUserRefreshTokens(string username, string refreshToken)
        {
            UserRefreshTokens item = _db.UserRefreshTokens.FirstOrDefault(
                u => u.UserName == username && u.RefreshToken == refreshToken
            );

            if (item != null)
            {
                _db.UserRefreshTokens.Remove(item);
            }
        }

        public UserRefreshTokens GetSavedRefreshTokens(string username, string refreshtoken)
        {
            return _db.UserRefreshTokens.FirstOrDefault(
                u => u.UserName == username && u.RefreshToken == refreshtoken && u.IsActived == true
            );
        }

        public async Task<User> IsValidUserAsync(string username, string password)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user is not null)
            {
                if (user.Password.Length > 0)
                {
                    if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                    {
                        return user;
                    }
                }
            }
            return null;
        }

        public int SaveCommit()
        {
            return _db.SaveChanges();
        }
    }
}
