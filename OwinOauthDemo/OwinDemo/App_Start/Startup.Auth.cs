using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using OwinDemo.Providers;
using OwinDemo.Models;

namespace OwinDemo
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new GoldenKeyAuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(3),        //设置accessToken的过期时间
                AllowInsecureHttp = true,
                RefreshTokenProvider = new GoldenKeyRefreshTokenProvider()
            };

            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}
