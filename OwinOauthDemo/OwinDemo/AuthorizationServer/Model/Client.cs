using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthorizationServer.Model
{
    public class Client
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Name { get; set; }
        public Int64 Active { get; set; }
        public Int64 RefreshTokenLifeTime { get; set; }
        public string DateAdded { get; set; }
    }
}