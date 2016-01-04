using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace OwinDemo
{
    public class GoldenKeyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.TryGetFormCredentials(out clientId, out clientSecret);

            //待续工作 搞清楚clientId、clientSecret的意义
            if (clientId == "1234" && clientSecret == "5678")       
            {
                context.Validated(clientId);
            }

            return base.ValidateClientAuthentication(context);
        }

        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, "iOS App"));      //待续工作 考察 "iOS App" 的意义
            var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
            context.Validated(ticket);

            return base.GrantClientCredentials(context);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);

            //待续工作 考察验证失败时的处理方式
            string ClaimTypes_Name;
            if (UserService.Login(context.UserName, context.Password))     //判断用户名、密码是否正确
            {
                ClaimTypes_Name = context.UserName;
            }
            else {  //如果 用户名、密码匹配失败，返回client access token
                ClaimTypes_Name = "client";
            }

            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, ClaimTypes_Name));
            var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
            context.Validated(ticket);

            await base.GrantResourceOwnerCredentials(context);
        }
    }
}