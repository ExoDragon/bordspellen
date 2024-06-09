using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace BordSpellenApi.Util;

public static class TokenHelper
{
    private static JwtSecurityToken DecodeToken(this StringValues token)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadJwtToken(token);
    }

    public static string GetEmailFromToken(this HttpRequest request)
    {
        var token = request.Headers[HeaderNames.Authorization];
        return token.DecodeToken().Claims.First(c => c.Type == ClaimTypes.Name).Value;
    }
}