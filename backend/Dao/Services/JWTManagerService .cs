using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using webapi.Dao.IServices;
using webapi.Model;

namespace webapi.Dao.Services
{
    public class JWTManagerService : IJWTManagerService
    {
        private readonly IConfiguration iconfiguration;

        public JWTManagerService(IConfiguration iconfiguration)
        {
            this.iconfiguration = iconfiguration;
        }

        public Tokens GenerateRefreshToken(string username, int roleId)
        {
            return GenerateJWTTokens(username, roleId);
        }

        public Tokens GenerateToken(string username, int roleId)
        {
            return GenerateJWTTokens(username, roleId);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var secretKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:SecretKey"]);
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken
            );

            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if (
                jwtSecurityToken == null
                || !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        // Generate token method
        private Tokens GenerateJWTTokens(string username, int roleId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:SecretKey"]);
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(
                        new Claim[]
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Aud, iconfiguration["JWT:Audience"]),
                            new Claim(JwtRegisteredClaimNames.Iss, iconfiguration["JWT:Issuer"]),
                            new Claim(ClaimTypes.Role, roleId.ToString())
                        }
                    ),
                    Expires = DateTime.Now.AddHours(3),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(secretKey),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var refreshToken = GenerateRfToken();
                return new Tokens
                {
                    AccessToken = tokenHandler.WriteToken(token),
                    RefreshToken = refreshToken
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // Generate refresh token method
        public string GenerateRfToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
