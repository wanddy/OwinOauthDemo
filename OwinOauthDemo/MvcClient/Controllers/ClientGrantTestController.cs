using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MvcClient.Controllers
{
    public class ClientGrantTestController : Controller
    {
        // GET: ClientGrantTest
        public ActionResult Index()
        {
            return View();
        }

        private HttpClient _httpClient;
        public HttpClient HtpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                    _httpClient.BaseAddress = new Uri("http://localhost:4282");     //发送请求时的基地址
                }
                return _httpClient;
            }
        }

        public string GetAccessToken()
        {
            var clientId = "e8eaf916-291d-44d1-aa65-dc501ad5cb3f";
            var clientSecret = "123456";
            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "password");
            parameters.Add("username", "goldenkey");
            parameters.Add("password", "123");

            //用http basic authentication header将clientId，clientSecret传给authentication server
            HtpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Basic",Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));

            string responseValue = HtpClient.PostAsync("/token", new FormUrlEncodedContent(parameters)).Result.Content.ReadAsStringAsync().Result;
            string acessToken = JObject.Parse(responseValue)["access_token"].Value<string>();
            HttpContext.Cache.Insert("accessToken", acessToken);

            return responseValue;
        }

        public string RefreshAccessToken()
        {
            var clientId = "e8eaf916-291d-44d1-aa65-dc501ad5cb3f";
            var clientSecret = "123456";
            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "refresh_token");
            parameters.Add("refresh_token", Request["refreshToken"]);

            HtpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));

            string responseValue = HtpClient.PostAsync("/token", new FormUrlEncodedContent(parameters)).Result.Content.ReadAsStringAsync().Result;
            string acessToken = JObject.Parse(responseValue)["access_token"].Value<string>();
            HttpContext.Cache.Insert("accessToken", acessToken);

            return responseValue;
        }

        public string GetResource()
        {
            string token = HttpContext.Cache.Get("accessToken").ToString();
            HtpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var responseValue = HtpClient.GetAsync("/api/values").Result.Content.ReadAsStringAsync().Result;
            return responseValue;
        }
    }
}