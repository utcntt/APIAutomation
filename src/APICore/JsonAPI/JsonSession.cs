using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICore;
using System.Net.Http;
using APICore.Helper;
using APICore.HttpCore;

namespace APICore.JsonAPI
{
    public class JsonSession : SessionBase
    {
        public JsonSession( string serverURL) : base(serverURL)
        {
            
        }

        /// <summary>
        /// Send a request 
        /// </summary>
        /// <typeparam name="T">Type of reponse data object</typeparam>
        /// <param name="method">API method</param>
        /// <returns></returns>
        public async Task<T> CallAsync<T>(APIMethod method)
        {
            HttpResponseMessage response = await this.CallAsync(method);
            T result = default(T);
            if(response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                if(string.IsNullOrEmpty(responseData))
                {
                    result = JsonHelper.ParseJSONString<T>(responseData);
                }
            }
            else
            {
                //To-do: write log or throw an exception.
            }
            return result;
        }
    }
}
