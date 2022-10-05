using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // Symmetric key is a type of ecryption where only one key is used to both encrypt and decrypt electronic information
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            // key will be stored in appsettings.json file.
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            // I'm adding one claim in this case
            var claims = new List<Claim>
            {
            // This is a type-value pair
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // I'm creating credentials with my secret key
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // I'm creating describing how the token is going to look.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            // this is going to handle my token.
            var tokenHandler = new JwtSecurityTokenHandler();

            // I'm creating token with the provided description
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Finally I'm returning the token as string
            return tokenHandler.WriteToken(token);
        }
    }
}