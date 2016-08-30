using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace APICore
{
    //An implememtation of a specific API
    public class APIMethod
    {
        #region Fields
        private string relativeUrl;
        private HttpMethod httpMethod;
        private Dictionary<string, object> urlParameter;
        private string requestData;
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        
        #endregion

        #region Contructors
        public APIMethod(string relativeUrl, HttpMethod httpMethod, Dictionary<string, object> urlParameter, string requestData)
        {
            this.relativeUrl = relativeUrl;
            this.httpMethod = httpMethod;
            this.urlParameter = urlParameter;
            this.requestData = requestData;
        }
        #endregion

        #region Public Elements
        public string RelativeUrl
        {
            get
            {
                return relativeUrl;
            }

            set
            {
                relativeUrl = value;
            }
        }

        public HttpMethod HttpMethod
        {
            get
            {
                return httpMethod;
            }

            set
            {
                httpMethod = value;
            }
        }

        public Dictionary<string, object> UrlParameter
        {
            get
            {
                return urlParameter;
            }

            set
            {
                urlParameter = value;
            }
        }

        /// <summary>
        /// To-do: Consider this property should be a byte array/ String Builder/String?
        /// </summary>
        public string RequestData
        {
            get
            {
                return requestData;
            }

            set
            {
                requestData = value;
            }
        }


        public Dictionary<string, string> Headers
        {
            get
            {
                return headers;
            }

            set
            {
                headers = value;
            }
        }
        #endregion
    }
}
