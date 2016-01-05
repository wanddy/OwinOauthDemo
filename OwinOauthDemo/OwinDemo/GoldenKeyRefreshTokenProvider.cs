using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinDemo
{
    public class GoldenKeyRefreshTokenProvider: AuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, string> _refreshTokens = new ConcurrentDictionary<string, string>();

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
        }
    }
}