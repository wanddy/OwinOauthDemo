using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OwinDemo.AuthorizationServer.DapperDal;
using OwinDemo.AuthorizationServer.Model;
using System.Threading.Tasks;

namespace OwinDemo.AuthorizationServer.Services
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