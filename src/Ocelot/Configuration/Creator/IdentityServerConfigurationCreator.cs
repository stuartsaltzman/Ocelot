using System;
using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Ocelot.Configuration.Provider;

namespace Ocelot.Configuration.Creator
{
    public static class IdentityServerConfigurationCreator
    {
	    public static class EnvironmentVariables
	    {
			public static string AdminUserName = "OCELOT_USERNAME";
			public static string AdminHash = "OCELOT_HASH";
			public static string AdminSalt = "OCELOT_SALT";
			public static string Certificate = "OCELOT_CERTIFICATE";
			public static string CertificatePassword = "OCELOT_CERTIFICATE_PASSWORD";
	    }
        
        public static IdentityServerConfiguration GetIdentityServerConfiguration()
        {
            var username = Environment.GetEnvironmentVariable(EnvironmentVariables.AdminUserName);
            var hash = Environment.GetEnvironmentVariable(EnvironmentVariables.AdminHash);
            var salt = Environment.GetEnvironmentVariable(EnvironmentVariables.AdminSalt);
            var credentialsSigningCertificateLocation = Environment.GetEnvironmentVariable(EnvironmentVariables.Certificate);
            var credentialsSigningCertificatePassword = Environment.GetEnvironmentVariable(EnvironmentVariables.CertificatePassword);

            return new IdentityServerConfiguration(
                "admin",
                false,
                SupportedTokens.Both,
                "secret",
                new List<string> { "admin", "openid", "offline_access" },
                "Ocelot Administration",
                true,
                GrantTypes.ResourceOwnerPassword,
                AccessTokenType.Jwt,
                false,
                new List<User>
                {
                    new User("admin", username, hash, salt)
                },
                credentialsSigningCertificateLocation,
                credentialsSigningCertificatePassword
            );
        }
    }
}
