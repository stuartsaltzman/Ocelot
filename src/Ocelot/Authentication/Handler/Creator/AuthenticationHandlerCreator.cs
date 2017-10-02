using System;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Responses;

namespace Ocelot.Authentication.Handler.Creator
{
    using Ocelot.Configuration;

    /// <summary>
    /// Cannot unit test things in this class due to use of extension methods
    /// </summary>
    public class AuthenticationHandlerCreator : IAuthenticationHandlerCreator
    {
	    private class EmptyStartup
	    {
		    public void ConfigureServices(IServiceCollection services) {}
		    public void Configure(IApplicationBuilder app) {}
	    }
	    
        public Response<RequestDelegate> Create(IApplicationBuilder app, AuthenticationOptions authenticationOptions)
        {
 
	        // HACK: This is a complete hack to simply test creating a new pipeline that has its own DI services registered
	        // with its own pipeline.
	        RequestDelegate CreatePipeline(Action<IServiceCollection> servicesConfiguration)
	        {
		        var webHost = new WebHostBuilder().UseKestrel()
			        .ConfigureServices(servicesConfiguration)
			        .UseStartup<EmptyStartup>()
			        .Build();
		        var serverFeatures = webHost.ServerFeatures;
		        var serviceProvider = app.ApplicationServices;
		        var appBuilderFactory = serviceProvider.GetRequiredService<IApplicationBuilderFactory>();
		        var branchBuilder = appBuilderFactory.CreateBuilder(serverFeatures);
		        var factory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

		        branchBuilder.Use(async (context, next) =>
		        {
			        using (var scope = factory.CreateScope())
			        {
				        context.RequestServices = scope.ServiceProvider;
				        await next();
			        }
		        });

		        return branchBuilder.Build();
	        }

	        RequestDelegate CreatePipelineFromConfig(AuthenticationOptions authOptions)
	        {
		        if (authOptions.Provider.ToLower() == "jwt")
		        {
			        var config = authOptions.Config as JwtConfig;
			        Action<IServiceCollection> addJwt = services =>
			        {
				        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					        .AddJwtBearer(options =>
					        {
						        options.Audience = config.Audience;
						        options.Authority = config.Authority;
					        });
			        };

			        return CreatePipeline(addJwt);
		        }
		        else
		        {
			        var config = authOptions.Config as IdentityServerConfig;
			        Action<IServiceCollection> addIdentity = services =>
			        {
				        services.AddAuthentication()
					        .AddIdentityServerAuthentication(options =>
					        {
						        options.Authority = config.ProviderRootUrl;
						        options.ApiName = config.ApiName;
						        options.RequireHttpsMetadata = config.RequireHttps;
						        // See https://github.com/IdentityServer/IdentityServer4.AccessTokenValidation/issues/87
						        //options.AllowedScopes = config.AllowedScopes;
						        options.SupportedTokens = SupportedTokens.Both;
						        options.ApiSecret = config.ApiSecret;
					        });
			        };
			        return CreatePipeline(addIdentity);
		        }
	        }

	        return new OkResponse<RequestDelegate>(CreatePipelineFromConfig(authenticationOptions));
	        
	        
	        //var builder = app.New();
	        
            //if (authOptions.Provider.ToLower() == "jwt")
            //{
               // var authenticationConfig = authOptions.Config as JwtConfig;
            
	            //  FIXME: this has moved to the IServiceCollection in Identity 2.0
                // builder.UseJwtBearerAuthentication(
                //     new JwtBearerOptions()
                //         {
                //             Authority = authenticationConfig.Authority,
                //             Audience = authenticationConfig.Audience
                //         });
            //}
            //else
            //{
            //    var authenticationConfig = authOptions.Config as IdentityServerConfig;
	            //  FIXME: this has moved to the IServiceCollection in Identity 2.0
				// 				 builder.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
				//                {
				//                    Authority = authenticationConfig.ProviderRootUrl,
				//                    ApiName = authenticationConfig.ApiName,
				//                    RequireHttpsMetadata = authenticationConfig.RequireHttps,
				//                    AllowedScopes = authOptions.AllowedScopes,
				//                    SupportedTokens = SupportedTokens.Both,
				//                    ApiSecret = authenticationConfig.ApiSecret
				//                });
            //}

            //var authenticationNext = builder.Build();
	        
            //return new OkResponse<RequestDelegate>(authenticationNext);
        }
    }
}