using System;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Ocelot.Responses;

namespace Ocelot.Authentication.Handler.Creator
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Ocelot.Configuration;

    using AuthenticationOptions = Configuration.AuthenticationOptions;

    /// <summary>
    /// Cannot unit test things in this class due to use of extension methods
    /// </summary>
    public class AuthenticationHandlerCreator : IAuthenticationHandlerCreator
    {
        public Response<RequestDelegate> Create(IApplicationBuilder app, AuthenticationOptions authOptions, IServiceCollection services)
        {
            var builder = app.New();

            if (authOptions.Provider.ToLower() == "jwt")
            {
                var authenticationConfig = authOptions.Config as JwtConfig;

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
                {
                    x.Authority = authenticationConfig.Authority;
                    x.Audience = authenticationConfig.Audience;
                });

            }
            else
            {
                var authenticationConfig = authOptions.Config as IdentityServerConfig;

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
                {
                    x.Authority = authenticationConfig.ProviderRootUrl;
                    x.Audience = authenticationConfig.ApiName;
                    x.RequireHttpsMetadata = authenticationConfig.RequireHttps;
                });
            }

            builder.UseAuthentication();

            var authenticationNext = builder.Build();

            return new OkResponse<RequestDelegate>(authenticationNext);
        }
    }
}