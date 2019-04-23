using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Alexa.NET.Request;
using authtesting.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace authtesting.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        //[Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(UserLogin objmodel)
        {
            var client = new RestClient("https://alexa-staging.evolutyzlabs.com/token");
            var request = new RestRequest(Method.POST);

            var amazonlink = "https://pitangui.amazon.com/spa/skill/account-linking-status.html?vendorId=M2TFGWVU88XERM";
            request.AddHeader("postman-token", "2acd8664-56af-cc3d-28aa-53f394ec1841");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("undefined", "grant_type=password&username=" + objmodel.Username + "&password=" + objmodel.Password, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //var result = response.Content.ReadAsStringAsync().Result;
            //GenerateTokens objData = new GenerateTokens();
            var token = JsonConvert.DeserializeObject(response.Content);

            //var accessToken = JsonConvert.DeserializeObject(response.Content);

            //var accessToken=((JObject)response)["access_token"].ToString();
            //request.AddHeader("grant", accessToken.ToString());
            //var redirUrl = amazonlink + "#state=" + Url.Encode(Request.QueryString["state"]) + "&access_token=" + Url.Encode(accessToken.ToString()) + "&token_type=Bearer";
            var redirUrl = amazonlink + "&state=" + Url.Encode(Request.QueryString["state"]) + "&access_token=" + Url.Encode(token.ToString()) + "&token_type=Bearer";

            return new RedirectResult(redirUrl);
        }
        [HttpPost]
       
        public async Task<ActionResult> LoginWithAlexa(UserLogin objmodel)
        {
            //var userName = User.Identity.Name;
            //send request to this application's /token endpoint with the username and a secret client
            //var clientSecret = "putSomeRandomStringHere";
            var redirUrl = string.Empty;
            var client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username",objmodel.Username },
                { "password",objmodel.Password},
                { "client_id", "4404bd0d-34a9-484b-a7d7-e1dcbc9ee55e" },
                {"client_secret", "af2a8378-9e23-4b2c-870f-6eb06a6b20ee" }
            };
            
            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(Request.Url.GetLeftPart(UriPartial.Authority) + "/token", content);
            if(response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);

                //grab the bearer token and send it to the amazon URL - redirect - use a fragment (#) to pass the parameters
                var amazonlink = "https://pitangui.amazon.com/spa/skill/account-linking-status.html?vendorId=M2TFGWVU88XERM";
                var accessToken = ((JObject)jsonResponse)["access_token"].ToString();
                //Random os = new Random();
                //var state = Convert.FromBase64String(os.Next(256).ToString());

                //var redirUrl = amazonlink + "#state=" + Url.Encode(Request.QueryString["state"]) + "&access_token=" + Url.Encode(accessToken) + "&token_type=Bearer";
                redirUrl = amazonlink + "#state=" + objmodel.Stateid + "&access_token=" + Url.Encode(accessToken) + "&token_type=Bearer";
                //var redirUrl = amazonlink + "#state=true" +  "&access_token=" + Url.Encode(accessToken) + "&token_type=Bearer";
                //amazon will make use of that bearer token
            }
            else
            {
                redirUrl = "http://alexa-staging.evolutyzlabs.com/Error.html";
            }



            return new RedirectResult(redirUrl);
        }
       
    }
    public class GenerateTokens
    {
        public string Token { get; set; }
        public string Username { get; set; }
    }
}