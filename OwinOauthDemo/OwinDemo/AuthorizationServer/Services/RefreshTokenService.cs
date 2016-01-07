using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DapperDal;
using OwinDemo.AuthorizationServer.Model;
using System.Threading.Tasks;

namespace OwinDemo.AuthorizationServer.Services
{
    public class RefreshTokenService
    {
        private RefreshTokenDal rfDal = new RefreshTokenDal();

        public async Task<bool> Save(RefreshToken refreshToken)
        {
            if (await rfDal.Insert(refreshToken) > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<RefreshToken> Get(string refreshTokenId)
        {
            RefreshToken refreshToken = (await rfDal.GetEntities()).Where(i=>i.Id==refreshTokenId).First();
            return refreshToken;
        }

        public async Task<bool> Remove(string refreshTokenId)
        {
            RefreshToken refreshToken =await this.Get(refreshTokenId);
            if (await rfDal.Delete(refreshToken) > 0)
            {
                return true;
            }
            return false;
        }

        public async Task RemoveByClientIdAndUserId(string clientId, string userId)
        {
            RefreshToken refreshToken = new RefreshToken()
            {
                ClientId = clientId,
                UserId = userId
            };
            await rfDal.DeleteByClientIdAndUserId(refreshToken);
        }
    }
}