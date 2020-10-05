using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace codesample
{
    public class ApiSampleCode
    {
        private static readonly string SubscriptionKey = "<Enter the Subscription Key here>";
        private static readonly string BaseUrl = "<Enter the Base Url here, for e.g. https://oat-api-customerengagement.platform.education.gov.uk>";
        private static string Refresh_token { get; set; }
        private RestClient sampleClient;
        private RestRequest sampleRequest;
       

        public ApiSampleCode()
        {
            sampleClient = new RestClient()
            {
                BaseUrl = new System.Uri(BaseUrl)
            };
        }

        public void GetOpenInfo()
        {
            // construct the GET request for our DfE Open Information endpoint 
            sampleRequest = new RestRequest()
            {
               Resource = "DfE/open-info",
               Method = Method.GET
            };

            // Add Subscription Key Header
            sampleRequest.AddHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);

            // Execute the query and retrieve results
            var queryResult = sampleClient.Execute(sampleRequest);
            HttpStatusCode statusCode = queryResult.StatusCode;
            int numericStatusCode = (int)statusCode;
        }

        public void GetApplicationInfo()
        {
            // construct the GET request for our DfE Application Information endpoint 
            sampleRequest = new RestRequest()
            {
                Resource = "DfE/application-info",
                Method = Method.GET
            };

            // Add Headers
            sampleRequest.AddHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);
            sampleRequest.AddHeader("authorisation", "Bearer " + GetAccessToken_AuthorisationCode());
            sampleRequest.AddHeader("accept", "application/json; charset=utf-8");

            // Execute the query and retrieve results
            var response = sampleClient.Execute(sampleRequest);
        }

        public void GetUserInfo()
        {
            // construct the GET request for our DfE Application Information endpoint 
            sampleRequest = new RestRequest()
            {
                Resource = "DfE/user-info",
                Method = Method.GET
            };  

            // Add Headers
            sampleRequest.AddHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);
            sampleRequest.AddHeader("authorisation", "Bearer " + GetAccessToken_ClientCredentials());
            sampleRequest.AddHeader("accept", "application/json; charset=utf-8");

            // Execute the query and retrieve results
            var response = sampleClient.Execute(sampleRequest);
        }

        private string GetAccessToken_ClientCredentials()
        {
            var tokenClient = new RestClient();
            var tokenRequest = new RestRequest()
            {
                Resource = "<token endpoint resource>",
                Method = Method.POST,
            };
            tokenRequest.AddParameter("grant_type", "client_credentials");
            tokenRequest.AddParameter("client_id", "<PASTE client id here>");
            tokenRequest.AddParameter("client_secret", "<PASTE client secret here>");
            tokenRequest.AddParameter("Content-Type", "application/x-www-form-urlencoded");

            // make call to get token
            IRestResponse response = tokenClient.Execute(tokenRequest);

            // extract access token and refresh token
            Refresh_token = JObject.Parse(response.Content).SelectToken("$..refresh_token").ToString();
            return JObject.Parse(response.Content).SelectToken("$..access_token").ToString();
        }

        private string GetAccessToken_AuthorisationCode()
        {
            var tokenClient = new RestClient();
            var tokenRequest = new RestRequest()
            {
                Resource = "<token endpoint resource>",
                Method = Method.POST,
            };
            tokenRequest.AddParameter("grant_type", "authorisation_code");
            tokenRequest.AddParameter("code", "<PASTE authorisation code here>");
            tokenRequest.AddParameter("client_id", "<PASTE client id here>");
            tokenRequest.AddParameter("client_secret", "<PASTE client secret here>");
            tokenRequest.AddParameter("redirect_URL", "<PASTE Redirect URL here>");

            // make call to get token
            IRestResponse response = tokenClient.Execute(tokenRequest);

            // extract access token and refresh token
            Refresh_token = JObject.Parse(response.Content).SelectToken("$..refresh_token").ToString();
            return JObject.Parse(response.Content).SelectToken("$..access_token").ToString();
        }

        private string RefreshAccessToken()
        {
            var tokenClient = new RestClient();
            var tokenRequest = new RestRequest()
            {
                Resource = "<token endpoint resource>",
                Method = Method.POST,
            };
            tokenRequest.AddParameter("grant_type", "refresh_token");
            tokenRequest.AddParameter("refresh_token", Refresh_token);
            tokenRequest.AddParameter("client_id", "<PASTE client id here>");
            tokenRequest.AddParameter("client_secret", "<PASTE client secret here>");
            tokenRequest.AddParameter("Content-Type", "application/x-www-form-urlencoded");

            // make call to get token
            IRestResponse response = tokenClient.Execute(tokenRequest);

            // extract token
            return JObject.Parse(response.Content).SelectToken("$..access_token").ToString();
        }
    }
}


