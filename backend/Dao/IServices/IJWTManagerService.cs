using System.Security.Claims;
using webapi.Model;

namespace webapi.Dao.IServices
{
    public interface IJWTManagerService
    {
        Tokens GenerateToken(string username, int roleId);
        Tokens GenerateRefreshToken(string username, int roleId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
