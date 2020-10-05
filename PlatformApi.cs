using RestSharp;
using System;
using System.Configuration;
using TT.API.Base.Library;
using TT.Core.BaseLayer.Core.Base.Library;
using RestSharp.Serializers.NewtonsoftJson;
using eapim_external_integration_tests.DataModels;
using TT.Core.BaseLayer.Core.Utilities;
using TT.Core.BaseLayer.Core.Helpers;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace eapim_external_integration_tests.ServiceObjects
{
    public class PlatformApi : ApiCore
    {
        private static readonly string BaseUri = ConfigurationManager.AppSettings["PlatformApi_BaseUrl_" + GlobalEnvironmentContext.RunEnvironment];
        private static readonly string SubscriptionKey = ConfigurationManager.AppSettings["PlatformApi_SubscriptionKey_" + GlobalEnvironmentContext.RunEnvironment];
        private static RestClient iRestClient;
        private static RestRequest iRestReq;

        public static string ApplicationSuffix { get; private set; }
        public static string ApplicationId { get; set; }
        public static string SubscriptionId { get; set; }
        public static string ApplicationName { get; private set; }
        public static string Key_Id { get; set; }

        public PlatformApi()
        {
            iRestClient = new RestClient()
            {
                BaseUrl = new Uri(BaseUri),
            };
            //iRestClient.UseNewtonsoftJson();
        }

        public string GetAllApis()
        {
            iRestReq = new RestRequest()
            {
                Resource = "GetAllApis",
                Method = Method.GET,
            };
            SetRequestHeader();
            ExecuteRestApiCall(iRestClient, iRestReq);
            CheckHttpStatusCodeRest("OK");
            RestSharpExtensions.RestResponse = RestSharpExtensions.RestResponse.Replace("response:", "");
            return GetNodeValueAtJsonPath(".value");
        }

        public string GetApisByTag(string tagName)
        {
            iRestReq = new RestRequest()
            {
                Resource = "apis",
                Method = Method.GET,
            };
            SetRequestHeader();
            iRestReq.AddQueryParameter("Tags", tagName);
            ExecuteRestApiCall(iRestClient, iRestReq);
            RestSharpExtensions.RestResponse = RestSharpExtensions.RestResponse.Replace("response:", "");
            return GetNodeValueAtJsonPath(".value");
        }

        public void RegisterApplication()
        {
            iRestReq = new RestRequest()
            {
                Resource = "RegisterApplication",
                Method = Method.POST,
            };
            SetRequestHeader();
            ApplicationSuffix = RandomDataGenerator.GenerateRandomNumber(5);
            var registerAppBody = new RegisterApplicationReqParams()
            {
                userName = "Auto tester" + ApplicationSuffix,
                userEmail = "Auto.tester" + ApplicationSuffix + ".gmail.com",
                userID = "Auto-tester" + ApplicationSuffix + "-id",
                organization = "dfe",
                role = "autotester",
                applicationName = "EapimAutotester" + ApplicationSuffix + "Application",
                description = "EapimAutotester" + ApplicationSuffix + "Application Description",
                redirectUri = "https://dev-developers-customerengagement.platform.education.gov.uk",
            };
            iRestReq.AddJsonBody(registerAppBody);
            ExecuteRestApiCall(iRestClient, iRestReq);
            ReportingHelper.ReportStepResult("Post Register Application Request to Platform API", "Passed", "Register Application request submitted to Platform Api with following details: " + JToken.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(registerAppBody)).ToString(Newtonsoft.Json.Formatting.Indented));
        }


        public void RegisterApplicationWithDuplicateName()
        {
            iRestReq = new RestRequest()
            {
                Resource = "RegisterApplication",
                Method = Method.POST,
            };
            SetRequestHeader();
            var registerAppBody = new RegisterApplicationReqParams()
            {
                userName = "Auto tester" + ApplicationSuffix,
                userEmail = "Auto.tester" + ApplicationSuffix + ".gmail.com",
                userID = "Auto-tester" + ApplicationSuffix + "-id",
                organization = "dfe",
                role = "autotester",
                applicationName = "EapimAutotester" + ApplicationSuffix + "Application",
                description = "EapimAutotester" + ApplicationSuffix + "Application Description",
                redirectUri = "https://dev-developers-customerengagement.platform.education.gov.uk",
            };
            iRestReq.AddJsonBody(registerAppBody);
            ExecuteRestApiCall(iRestClient, iRestReq);
            ReportingHelper.ReportStepResult("Post Register Application Request to Platform API", "Passed", "Register Application request submitted to Platform Api with following details: " + JToken.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(registerAppBody)).ToString(Newtonsoft.Json.Formatting.Indented));
        }

        public void GetApplicationById(string appId)
        {
            iRestReq = new RestRequest()
            {
                Resource = "ApplicationByID",
                Method = Method.GET,
            };
            SetRequestHeader();
            iRestReq.AddQueryParameter("applicationId", appId);
            ExecuteRestApiCall(iRestClient, iRestReq);
            ReportingHelper.ReportStepResult("Get ApplicationById Request to Platform API", "Passed", "Get ApplicationById request submitted to Platform Api with following application Id: " + appId);
        }

        public void ValidateApplicationDetails(string appId)
        {
            CheckTextExistsAtJsonPath(".applicationId", appId);
            CheckTextExistsAtJsonPath(".applicationName", "dfedeveloperhub-Auto-tester" + ApplicationSuffix + "-id-dfe-EapimAutotester" + ApplicationSuffix + "Application");
            CheckTextExistsAtJsonPath(".description", "EapimAutotester" + ApplicationSuffix + "Application Description");
        }

        public void UpdateApplication()
        {
            iRestReq = new RestRequest()
            {
                Resource = "UpdateApplication",
                Method = Method.POST,
            };
            SetRequestHeader();
            var updateAppBody = new UpdateApplicationRequestParams()
            {
                userName = "Auto tester" + ApplicationSuffix,
                userEmail = "Auto.tester" + ApplicationSuffix + ".gmail.com",
                userID = "Auto-tester" + ApplicationSuffix + "-id",
                applicationId = ApplicationId,
                description = "EapimAutotester" + ApplicationSuffix + "Application Description",
                web = new URIS()
                {
                    redirectUris = new string[]
                    {
                        "https://dev-developers-customerengagement.platform.education.gov.uk",
                        "https://test-developers-customerengagement.platform.education.gov.uk",
                    }
                },
            };
            iRestReq.AddJsonBody(updateAppBody);
            ExecuteRestApiCall(iRestClient, iRestReq);
            ReportingHelper.ReportStepResult("Post Update Application Request to Platform API", "Passed", "Update Application request submitted to Platform Api with following details: " + JToken.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(updateAppBody)).ToString(Newtonsoft.Json.Formatting.Indented));
        }

        public void DeleteApplication()
        {
            iRestReq = new RestRequest()
            {
                Resource = "DeleteApplication",
                Method = Method.DELETE,
            };
            SetRequestHeader();
            var deleteAppBody = new DeleteApplicationRequestParams()
            {
                userName = "Auto tester" + ApplicationSuffix,
                userEmail = "Auto.tester" + ApplicationSuffix + ".gmail.com",
                userID = "Auto-tester" + ApplicationSuffix + "-id",
                applicationId = ApplicationId,
            };
            iRestReq.AddJsonBody(deleteAppBody);
            ExecuteRestApiCall(iRestClient, iRestReq);
            ReportingHelper.ReportStepResult("Post Delete Application Request to Platform API", "Passed", "Delete Application request submitted to Platform Api with following details: " + JToken.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(deleteAppBody)).ToString(Newtonsoft.Json.Formatting.Indented));
        }

        public void CreateSubscription(string envName, string applicationId, string apiName)
        {
            GetSubscriptions(ApplicationId);
            Thread.Sleep(2000);
            iRestReq = new RestRequest()
            {
                Resource = "subscription",
                Method = Method.POST,
            };
            SetRequestHeader();
            var createSubBody = new CreateSubscriptionRequestParams()
            {
                applicationId = applicationId,
                apiName = apiName,
                environment = envName,              
            };
            iRestReq.AddJsonBody(createSubBody);
            ExecuteRestApiCall(iRestClient, iRestReq);
            ReportingHelper.ReportStepResult("Post Create Subscription Request to Platform API", "Passed", "Create Subscription request submitted to Platform Api with following details: " + JToken.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(createSubBody)).ToString(Newtonsoft.Json.Formatting.Indented));
        }

        public void GetSubscriptions(string appId)
        {
            iRestReq = new RestRequest()
            {
                Resource = "Subscriptions",
                Method = Method.GET,
            };
            SetRequestHeader();
            iRestReq.AddQueryParameter("applicationId", appId);
            ExecuteRestApiCall(iRestClient, iRestReq);
            ReportingHelper.ReportStepResult("GetSubscriptions Request to Platform API", "Passed", "GetSubscriptions request submitted to Platform Api with following application Id: " + appId);
        }

        public void DeleteSubscription(string envName)
        {
            iRestReq = new RestRequest()
            {
                Resource = "Subscription",
                Method = Method.DELETE,
            };
            SetRequestHeader();
            var deleteSubBody = new DeleteSubscriptionRequestParams()
            {
                subscriptionId = SubscriptionId,
                environment = envName,
            };
            iRestReq.AddJsonBody(deleteSubBody);
            ExecuteRestApiCall(iRestClient, iRestReq);
            ReportingHelper.ReportStepResult("Post Delete Subscription Request to Platform API", "Passed", "Delete Subscription request submitted to Platform Api with following details: " + JToken.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(deleteSubBody)).ToString(Newtonsoft.Json.Formatting.Indented));
        }

        public void GenerateApplicationSecret(string type)
        {
            iRestReq = new RestRequest()
            {
                Resource = "GenerateApplicationSecret",
                Method = Method.POST,
            };
            SetRequestHeader();
            var generateSecretBody = new GenerateApplicationSecretRequestParams()
            {
                userName = "Auto tester" + ApplicationSuffix,
                userEmail = "Auto.tester" + ApplicationSuffix + ".gmail.com",
                userID = "Auto-tester" + ApplicationSuffix + "-id",
                applicationId = ApplicationId,
                KeyId = Key_Id,
                KeyDisplayName = type,
            };
            iRestReq.AddJsonBody(generateSecretBody);
            ExecuteRestApiCall(iRestClient, iRestReq);
            ReportingHelper.ReportStepResult("Post Generate Application Secret Request to Platform API", "Passed", "Generate Application secret submitted to Platform Api with following details: " + JToken.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(generateSecretBody)).ToString(Newtonsoft.Json.Formatting.Indented));
        }

        private void SetRequestHeader()
        {
           iRestReq.AddHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);
        }
    }
}
