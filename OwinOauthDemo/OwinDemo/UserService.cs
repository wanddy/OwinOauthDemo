using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OwinDemo
{
    public class UserService
    {
        //模拟username、pwd验证，规则：从数据库进行用户username、pwd匹配，有该用户，就返回true，否则返回false
        public static bool Login(string uid,string pwd)     
        {
            if (uid == "goldenkey"&&pwd == "123")
            {
                return true;
            }
            return false;
        }
    }
}