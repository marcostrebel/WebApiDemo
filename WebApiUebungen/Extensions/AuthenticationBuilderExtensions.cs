using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiUebungen.Settings;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder ConfigureJwtAuthentication(this AuthenticationBuilder builder, JwtBearerSettings settings)
        {
            return builder.AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = settings.ValidateIssuer,
                     ValidateAudience = settings.ValidateAudience,
                     ValidateLifetime = settings.ValidateLifetime,
                     ValidateIssuerSigningKey = settings.ValidateIssuerSigningKey,
                     ValidIssuer = settings.Issuer,
                     ValidAudience = settings.Audience,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)),
                     //LifetimeValidator = CustomLifetimeValidator
                 };
             });
        }
    }
}
