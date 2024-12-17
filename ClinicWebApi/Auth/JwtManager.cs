using ClinicWebApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClinicWebApi.Auth
{
    public interface IJwtManager
    {
        Token GetToken(UserModel user);
    }

    public class JwtManager : IJwtManager
    {
        private readonly IConfiguration _iconfiguration;

        public JwtManager(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
        }

        public Token GetToken(UserModel user)
        {
            if (user == null || user.Id == 0)
                throw new ArgumentNullException(nameof(user), "User or User ID is invalid.");

            var tokenKey = _iconfiguration["JWT:Key"];
            if (string.IsNullOrEmpty(tokenKey))
                throw new Exception("JWT key is not configured.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(tokenKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserID", user.Id.ToString()),
                    new Claim("RoleID", user.Role_id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            try
            {
                var tokenData = tokenHandler.CreateToken(tokenDescriptor);
                var token = new Token
                {
                    AccessToken = tokenHandler.WriteToken(tokenData)
                };

                return token;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating JWT: {ex.Message}", ex);
            }
        }
    }

    public class Token
    {
        public string? AccessToken { get; set; }
    }
}