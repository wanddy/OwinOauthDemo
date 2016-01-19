using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AuthorizationServer.DapperDal;
using AuthorizationServer.Model;
using System.Threading.Tasks;

namespace AuthorizationServer.Services
{
    public class ClientService
    {
        private ClientDal clientDal = new ClientDal();

        public async Task<Client> GetClientById(string clientId)
        {
            Client client = (clientDal.GetEntities(m=>true)).Where(i => i.Id == clientId).First();
            return client;
        }

        public async Task<bool> Save(Client client)
        {
            if (clientDal.Insert(client) > 0)
            {
                return true;
            }
            return false;
        }
    }
}