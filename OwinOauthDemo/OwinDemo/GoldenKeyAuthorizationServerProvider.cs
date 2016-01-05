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
            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { "as:client_id", context.ClientId }
                });
            var ticket = new AuthenticationTicket(oAuthIdentity, props);
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

        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];  //反序列化得到的clientId
            var currentClient = context.ClientId;   //新请求中的clientId

            if (originalClient != currentClient)
            {
                context.Rejected();
                return;
            }

            var newId = new ClaimsIdentity(context.Ticket.Identity);    //新的refreshId的ticketId
            newId.AddClaim(new Claim("newClaim", "refreshToken"));  //。。。

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);     //根据旧的ticket创建新的ticket
            context.Validated(newTicket);   //将新的 ticket 设置为当前context的ticket（将就旧的覆盖掉了）

            await base.GrantRefreshToken(context);
        }
    }
}