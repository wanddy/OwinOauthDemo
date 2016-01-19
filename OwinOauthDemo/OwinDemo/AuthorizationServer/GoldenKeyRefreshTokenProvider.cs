using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using AuthorizationServer.DapperDal;
using AuthorizationServer.Services;
using AuthorizationServer.Model;

namespace AuthorizationServer
{
    public class GoldenKeyRefreshTokenProvider: AuthenticationTokenProvider
    {
        RefreshTokenService _refreshTokenService = new RefreshTokenService();
        public override async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            if (string.IsNullOrEmpty(context.Ticket.Identity.Name)) return;     //client credentials grant模式不生成refresh_Token

            var clietId = context.OwinContext.Get<string>("as:client_id"); //HttpContext.Current.Request["client_id"];
            if (string.IsNullOrEmpty(clietId)) return;

            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime"); ; //context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");
            if (string.IsNullOrEmpty(refreshTokenLifeTime)) return;

            //generate access token
            RandomNumberGenerator cryptoRandomDataGenerator = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[50];
            cryptoRandomDataGenerator.GetBytes(buffer);
            var refreshTokenId = Convert.ToBase64String(buffer).TrimEnd('=').Replace('+', '-').Replace('/', '_');

            DateTime issuedUtc = DateTime.UtcNow;
            DateTime expireUtc = DateTime.UtcNow.AddSeconds(Convert.ToDouble(refreshTokenLifeTime));

            var refreshToken = new RefreshToken()
            {
                Id = refreshTokenId,
                ClientId = new Guid(clietId).ToString(),
                UserId = context.Ticket.Identity.Name,
                IssuedUtc = issuedUtc.ToString(),
                ExpiresUtc = expireUtc.ToString(),
               // ProtectedTicket = context.SerializeTicket()
            };

            context.Ticket.Properties.IssuedUtc = issuedUtc;
            context.Ticket.Properties.ExpiresUtc = expireUtc;

            //先设置过期时间，再序列化protectedTicket，出处：http://www.cnblogs.com/dudu/p/4721797.html
            refreshToken.ProtectedTicket = context.SerializeTicket();

            //删除之前refreshToken，出处：http://www.cnblogs.com/dudu/p/4679592.html
            await _refreshTokenService.RemoveByClientIdAndUserId(clietId, context.Ticket.Identity.Name);        

            if (await _refreshTokenService.Save(refreshToken))
            {
                context.SetToken(refreshTokenId);
            }
        }



        public override async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            RefreshToken refreshToken = await _refreshTokenService.Get(context.Token);

            if (refreshToken != null)
            {
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                var result = _refreshTokenService.Remove(refreshToken);
            }
        }
    }
}