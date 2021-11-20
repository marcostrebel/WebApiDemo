using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiUebungen.Settings;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static AuthenticationBuilder ConfigureDefaultAuthentication(this IServiceCollection services, AuthenticationSettings settings)
        {
            return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = settings.DefaultScheme;
                options.DefaultChallengeScheme = settings.DefaultScheme;
            });
        }

        //public static void ConfigureAuthentication(this IServiceCollection services, AuthenticationSettings settings)
        //{
        //    if (settings is null || !settings.Enable)
        //        return;

        //    var configurator = new AuthenticationConfigurator(settings);

        //    var authBuilder = services.AddAuthentication(configurator.ConfigureAuthentication);

        //    if (settings.Jwt.Enable)
        //    {
        //        authBuilder.AddJwtBearer(configurator.ConfigureJwtBearer);

        //        services.AddAuthorization(configurator.ConfigureAuthorizationOptions);
        //    }
        //}
    }
}
