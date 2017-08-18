using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Ocelot.Responses;

namespace Ocelot.Authentication.Handler.Creator
{
    using Microsoft.Extensions.DependencyInjection;
    using AuthenticationOptions = Configuration.AuthenticationOptions;

    public interface IAuthenticationHandlerCreator
    {
        Response<RequestDelegate> Create(IApplicationBuilder app, AuthenticationOptions authOptions, IServiceCollection services);
    }
}
