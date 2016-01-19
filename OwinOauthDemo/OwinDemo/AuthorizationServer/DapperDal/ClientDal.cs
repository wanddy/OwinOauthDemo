using DapperDal;
using OwinDemo.AuthorizationServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinDemo.AuthorizationServer.DapperDal
{
    public class ClientDal : BaseDal<Client>
    {
        public override void SetSqlCUD()
        {
            sqlCUD.Add = "insert into Client VALUES(@Id,@Secret,@Name,@IsActive,@RefreshTokenLifeTime,@DateAdded);";
            sqlCUD.Update = "update Client set Secret=@Secret,Name=@Name,IsActive=@IsActive,RefreshTokenLifeTime=@RefreshTokenLifeTime, DateAdded=@DateAdded where Id=@Id";
            sqlCUD.Delete = "delete from Client where Id=@Id";
        }
    }
}