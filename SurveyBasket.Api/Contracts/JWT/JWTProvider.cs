using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Api.Contracts.JWT
{
    public class JWTProvider(IOptions<JWTOptions> options): IJWTProvider
    {
        private readonly JWTOptions jWTOptions = options.Value;
        public (string Token, int ExpiresIn) GenerateToken(ApplicationUser user)
        {
            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub,user.Id),
                new(JwtRegisteredClaimNames.Email,user.Email!),
                 new(JwtRegisteredClaimNames.GivenName,user.FirstName),
                  new(JwtRegisteredClaimNames.FamilyName,user.LastName),
                   new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
              ];
            var signingkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTOptions.key));
            var signingCredentials = new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256);

            var Token = new JwtSecurityToken
           (
                issuer : jWTOptions.Issuer,
                audience: jWTOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jWTOptions.ExpiryMinutes),
                signingCredentials: signingCredentials
           );

            return (Token: new JwtSecurityTokenHandler().WriteToken(Token),ExpiresIn:jWTOptions.ExpiryMinutes*60);
        }

        public string? ValidateToken(string token)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var symmetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTOptions.key));
            try
            {
                tokenhandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = symmetrickey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwttoken = (JwtSecurityToken)validatedToken;
                return jwttoken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            }
            catch
            {
                return null;
            }

        }
    }
}
