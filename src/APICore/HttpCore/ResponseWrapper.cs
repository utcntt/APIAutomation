using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APICore.HttpCore
{
    /// <summary>
    /// A wrapper class of a http response
    /// </summary>
    public class ResponseWrapper
    {
        public ResponseWrapper(HttpResponseMessage response)
        {
            this.Response = response;
            this.HttpCode = response.StatusCode;
            this.ResponseData = response.Content == null ? Task.FromResult<string>(null) :  response.Content.ReadAsStringAsync();
            this.Headers = response.Headers;
        }

        /// <summary>
        /// Gets the server response as String
        /// </summary>
        /// <returns></returns>
        public Task<string> ResponseData { get; }

        /// <summary>
        /// Gets the Http Code
        /// </summary>
        /// <returns></returns>
        public HttpStatusCode HttpCode { get; }

        /// <summary>
        /// Get the response of the Handler
        /// </summary>
        public HttpResponseMessage Response { get; }

        public HttpResponseHeaders Headers { get; }
    }
}
