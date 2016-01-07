﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using OwinDemo.AuthorizationServer.Services;

namespace OwinDemo
{
    public class GoldenKeyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private ClientService clientService = new ClientService();
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            
            string clientId;
            string clientSecret;
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret)) { return; }

            var client = await clientService.GetClientById(context.ClientId);
            if (client == null) { return; }
            if (client.Secret != clientSecret) { return; }
            
            context.OwinContext.Set<string>("as:client_id", client.Id);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());
            context.Validated(client.Id);
        }

        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            context.Validated(oAuthIdentity);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            
            if (!UserService.Login(context.UserName, context.Password))     //判断用户名、密码是否正确
            {
                return;
            }

            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            context.Validated(oAuthIdentity);
        }

        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var newId = new ClaimsIdentity(context.Ticket.Identity);    //新的refreshId的ticketId
            newId.AddClaim(new Claim("newClaim", "refreshToken"));  //。。。

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);     //根据旧的ticket创建新的ticket
            context.Validated(newTicket);   //将新的 ticket 设置为当前context的ticket（将就旧的覆盖掉了）
        }
    }
}