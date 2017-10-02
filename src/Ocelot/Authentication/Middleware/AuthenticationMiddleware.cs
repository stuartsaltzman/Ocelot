﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Ocelot.Authentication.Handler.Factory;
using Ocelot.Configuration;
using Ocelot.Errors;
using Ocelot.Infrastructure.Extensions;
using Ocelot.Infrastructure.RequestData;
using Ocelot.Logging;
using Ocelot.Middleware;

namespace Ocelot.Authentication.Middleware
{
    public class AuthenticationMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _app;
        private readonly IAuthenticationHandlerFactory _authHandlerFactory;
        private readonly IOcelotLogger _logger;

        public AuthenticationMiddleware(RequestDelegate next,
            IApplicationBuilder app,
            IRequestScopedDataRepository requestScopedDataRepository,
            IAuthenticationHandlerFactory authHandlerFactory,
            IOcelotLoggerFactory loggerFactory)
            : base(requestScopedDataRepository)
        {
            _next = next;
            _authHandlerFactory = authHandlerFactory;
            _app = app;
            _logger = loggerFactory.CreateLogger<AuthenticationMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsAuthenticatedRoute(DownstreamRoute.ReRoute))
            {
                _logger.LogDebug($"{context.Request.Path} is an authenticated route. {MiddlewareName} checking if client is authenticated");

	            // IAuthenticationHandlerFactory creates a new pipeline via IApplicationBuilder.New. We should create the pipeline once
	            // and cache it.
                var authenticationHandler = _authHandlerFactory.Get(_app, DownstreamRoute.ReRoute.AuthenticationOptions);

                if (authenticationHandler.IsError)
                {
                    _logger.LogError($"Error getting authentication handler for {context.Request.Path}. {authenticationHandler.Errors.ToErrorString()}");
                    SetPipelineError(authenticationHandler.Errors);
                    return;
                }
	            
	            // HttpContext passed to the newly generated middleware pipeline
                await authenticationHandler.Data.Handler.Handle(context);


                if (context.User.Identity.IsAuthenticated)
                {
                    _logger.LogDebug($"Client has been authenticated for {context.Request.Path}");
                    await _next.Invoke(context);
                }
                else
                {
                    var error = new List<Error>
                    {
                        new UnauthenticatedError(
                            $"Request for authenticated route {context.Request.Path} by {context.User.Identity.Name} was unauthenticated")
                    };

                    _logger.LogError($"Client has NOT been authenticated for {context.Request.Path} and pipeline error set. {error.ToErrorString()}");
                    SetPipelineError(error);
                    return;
                }
            }
            else
            {
                _logger.LogTrace($"No authentication needed for {context.Request.Path}");

                await _next.Invoke(context);
            }
        }

        private static bool IsAuthenticatedRoute(ReRoute reRoute)
        {
            return reRoute.IsAuthenticated;
        }
    }
}

