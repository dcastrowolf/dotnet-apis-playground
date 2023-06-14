using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Identity.Mock.Token
{
    public static class TokenEndpoint
    {
        private static readonly TimeSpan TokenLifeTime = TimeSpan.FromHours(8);

        public static IEndpointRouteBuilder MapCreateToken(this IEndpointRouteBuilder app, IConfiguration configuration)
        {
            var TokenSecret = configuration["BooksApiSecret"]!;

            app.MapPost("/token", (TokenGenerationRequest request) =>
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(TokenSecret);

                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Sub, request.Email),
                    new(JwtRegisteredClaimNames.Email, request.Email),
                    new("userid", request.UserId.ToString())
                };

                foreach (var claimPair in request.CustomClaims)
                {
                    var jsonElement = (JsonElement)claimPair.Value;
                    var valueType = jsonElement.ValueKind switch
                    {
                        JsonValueKind.True => ClaimValueTypes.Boolean,
                        JsonValueKind.False => ClaimValueTypes.Boolean,
                        JsonValueKind.Number => ClaimValueTypes.Double,
                        _ => ClaimValueTypes.String
                    };

                    var claim = new Claim(claimPair.Key, claimPair.Value.ToString()!, valueType);
                    claims.Add(claim);
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.Add(TokenLifeTime),
                    Issuer = "https://id.books.com",
                    Audience = "https://books.danielitus.com",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                var jwt = tokenHandler.WriteToken(token);
                return Results.Ok(jwt);
            });
            return app;
        }
    }
}
