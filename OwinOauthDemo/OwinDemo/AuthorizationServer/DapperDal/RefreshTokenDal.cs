using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OwinDemo.AuthorizationServer.Model;
using System.Data;
using Dapper;

namespace DapperDal
{
    class RefreshTokenDal:BaseDal<RefreshToken>
    {
        public override void SetQuerys()
        {
            querys.GetEntities = "select * from RefreshToken";
            querys.Add = "insert into RefreshToken VALUES(@Id,@UserName,@ClientId,@IssuedUtc,@ExpiresUtc,@ProtectedTicket);";
            querys.Update = "update RefreshToken set Id=@Id,IssuedUtc=@IssuedUtc,ExpiresUtc=@ExpiresUtc,ProtectedTicket=@ProtectedTicket where UserName=@UserName and ClientId=@ClientId";
            querys.Delete = "delete from RefreshToken where UserName=@UserName and ClientId=@ClientId";
        }

        public async Task<int> DeleteByClientIdAndUserId(RefreshToken refreshToken)
        {
            using (IDbConnection conn = GetOpenConnection())
            {
                string query = "delete from RefreshToken where UserName=@UserName and ClientId=@ClientId";
                int row = conn.Execute(query, refreshToken);
                return row;
            }
        }
    }
}
