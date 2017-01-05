using APICore.HttpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QnAMaker.Test.Model;
using APICore.Helper;
using System.Net.Http;

namespace QnAMaker.Test.API
{
    public class AnswerAPI
    {
        private static SessionBase session = null;
        protected static SessionBase Session
        {
            get
            {
                if(session == null)
                {
                    string serverUrl = $"{ QnAMarkerConfiguration.ServerURL}/knowledgebases/{QnAMarkerConfiguration.KnowledgeBaseID}";
                    session = new SessionBase(serverUrl);
                    session.MasterHeader.Add("Ocp-Apim-Subscription-Key", QnAMarkerConfiguration.SubscriptionKey);
                    //session.MasterHeader.Add("Content-Type", "application/json");
                }
                return session;
            }
        }
        public const string GenerateAnswerURL = "/generateAnswer";
        public static async Task<AnswerResponseData> GenerateAnswerAsync (QuestionRequest question)
        {
            string requestData = JsonHelper.GenerateJSONString(question);
            APIMethod apiMethod = new APIMethod(GenerateAnswerURL, HttpMethod.Post, null, requestData);
            apiMethod.Headers = new Dictionary<string, string>() { { "Content-Type", "application/json" } };
            HttpResponseMessage httpResponse = await Session.CallAsync(apiMethod);
            ResponseWrapper response = new ResponseWrapper(httpResponse);
            if(response.HttpCode == System.Net.HttpStatusCode.OK)
            {
                AnswerResponseData data = JsonHelper.ParseJSONString<AnswerResponseData>(await response.ResponseData);
                return data;
            }
            return null;
        }
    }
}
