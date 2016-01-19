using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AuthorizationServer.DapperDal;
using AuthorizationServer.Model;
using System.Threading.Tasks;

namespace AuthorizationServer.Services
{
    public class RefreshTokenService
    {
        private RefreshTokenDal rfDal = new RefreshTokenDal();

        public async Task<bool> Save(RefreshToken refreshToken)
        {
            if (rfDal.Insert(refreshToken) > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<RefreshToken> Get(string refreshTokenId)
        {
            RefreshToken refreshToken = (rfDal.GetEntities(i => i.Id == refreshTokenId)).First();
            return refreshToken;
        }

        public async Task<bool> Remove(RefreshToken refreshToken)
        {
            if (rfDal.Delete(refreshToken) > 0)
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
            rfDal.DeleteByClientIdAndUserId(refreshToken);
        }
    }
}