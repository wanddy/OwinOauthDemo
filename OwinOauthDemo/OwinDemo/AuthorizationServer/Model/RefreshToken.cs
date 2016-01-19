using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationServer.Model
{
    public class RefreshToken
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string ClientId { get; set; }

        public string IssuedUtc { get; set; }

        public string ExpiresUtc { get; set; }

        public string ProtectedTicket { get; set; }
    }
}
