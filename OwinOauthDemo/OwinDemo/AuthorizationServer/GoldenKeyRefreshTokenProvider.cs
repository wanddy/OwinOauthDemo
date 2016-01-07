using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using DapperDal;
using OwinDemo.AuthorizationServer.Services;
using OwinDemo.AuthorizationServer.Model;

namespace OwinDemo
{
    public class GoldenKeyRefreshTokenProvider: AuthenticationTokenProvider
    {
        //private static ConcurrentDictionary<string, string> _refreshTokens = new ConcurrentDictionary<string, string>();

        /*
        public override void Create(AuthenticationTokenCreateContext context) 
        {
            string tokenValue = Guid.NewGuid().ToString("n");       //生成新的refresh token

            context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;                  //？这个是设置谁的有效时间
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(60);

            _refreshTokens[tokenValue] = context.SerializeTicket();     // 将刷新access token需要的ticket信息序列化，并保存

            context.SetToken(tokenValue);   //设置refreshToken
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            string value; 
            if (_refreshTokens.TryRemove(context.Token, out value))     //删除原来的原来的token
            {
                context.DeserializeTicket(value);       //反序列化，得到刷新access token需要的ticket信息
            }
        }*/

        //-----------------------------------------------------------------------------------------

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

            var refreshToken = new RefreshToken()
            {
                Id = refreshTokenId,
                ClientId = new Guid(clietId).ToString(),
                UserId = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddSeconds(Convert.ToDouble(refreshTokenLifeTime)),
                ProtectedTicket = context.SerializeTicket()
            };

            context.Ticket.Properties.IssuedUtc = refreshToken.IssuedUtc;           //这两句代码的意思：设置accessToken or refreshtoken
            context.Ticket.Properties.ExpiresUtc = refreshToken.ExpiresUtc;

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
                var result = _refreshTokenService.Remove(context.Token);
            }
        }
    }
}