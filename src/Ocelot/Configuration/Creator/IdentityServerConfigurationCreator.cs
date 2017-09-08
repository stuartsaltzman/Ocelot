using System;
using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using Ocelot.Configuration.Provider;

namespace Ocelot.Configuration.Creator
{
    public static class IdentityServerConfigurationCreator
    {
        public static String OCELOT_ADMIN_USERNAME_ENV_NAME = "OCELOT_USERNAME";
        public static String OCELOT_ADMIN_HASH_ENV_NAME = "OCELOT_HASH";
        public static String OCELOT_ADMIN_SALT_ENV_NAME = "OCELOT_SALT";
        public static String OCELOT_CERTIFICATE_ENV_NAME = "OCELOT_CERTIFICATE";
        public static String OCELOT_CERTIFICATE_PASSWORD_ENV_NAME = "OCELOT_CERTIFICATE_PASSWORD";

        public static IdentityServerConfiguration GetIdentityServerConfiguration()
        {
            var username = Environment.GetEnvironmentVariable(OCELOT_ADMIN_USERNAME_ENV_NAME);
            var hash = Environment.GetEnvironmentVariable(OCELOT_ADMIN_HASH_ENV_NAME);
            var salt = Environment.GetEnvironmentVariable(OCELOT_ADMIN_SALT_ENV_NAME);
            var credentialsSigningCertificateLocation = Environment.GetEnvironmentVariable(OCELOT_CERTIFICATE_ENV_NAME);
            var credentialsSigningCertificatePassword = Environment.GetEnvironmentVariable(OCELOT_CERTIFICATE_PASSWORD_ENV_NAME);

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
