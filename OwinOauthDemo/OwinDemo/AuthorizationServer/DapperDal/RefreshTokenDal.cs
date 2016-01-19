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
        public override void SetSqlCUD()
        {
            sqlCUD.Add = "insert into RefreshToken VALUES(@Id,@UserId,@ClientId,@IssuedUtc,@ExpiresUtc,@ProtectedTicket);";
            sqlCUD.Update = "update RefreshToken set Id=@Id,IssuedUtc=@IssuedUtc,ExpiresUtc=@ExpiresUtc,ProtectedTicket=@ProtectedTicket where UserId=@UserId and ClientId=@ClientId";
            sqlCUD.Delete = "delete from RefreshToken where UserId=@UserId and ClientId=@ClientId";
        }

        public int DeleteByClientIdAndUserId(RefreshToken refreshToken)
        {
            using (IDbConnection conn = GetOpenConnection())
            {
                string query = "delete from RefreshToken where UserId=@UserId and ClientId=@ClientId";
                int row = conn.Execute(query, refreshToken);
                return row;
            }
        }
    }
}
