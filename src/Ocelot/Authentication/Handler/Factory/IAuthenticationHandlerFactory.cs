using Microsoft.AspNetCore.Builder;
using Ocelot.Responses;

namespace Ocelot.Authentication.Handler.Factory
{
    using Microsoft.Extensions.DependencyInjection;
    using AuthenticationOptions = Configuration.AuthenticationOptions;

    public interface IAuthenticationHandlerFactory
    {
        Response<AuthenticationHandler> Get(IApplicationBuilder app, AuthenticationOptions authOptions, IServiceCollection services);
    }
}
