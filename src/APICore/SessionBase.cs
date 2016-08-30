using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APICore
{
    /// <summary>
    /// SessionBase contains all implementations to setup a new connection with a Server
    /// </summary>
    public class SessionBase
    {
        #region Private elements
        private string serverURL;
        //private Dictionary<string, string> header;
        private Encoding encoding = Encoding.UTF8;
        private HttpClient httpClient;
        private CookieContainer cookieContainer = new CookieContainer();
        private HttpClientHandler clientHandler;

        private HttpRequestMessage GetRequestMessage(APIMethod method)
        {
            string url = this.serverURL + method.RelativeUrl;

            if (method.UrlParameter != null && method.UrlParameter.Count > 0)
            {
                url += Util.FormatURLParameters(method.UrlParameter);
            }
            HttpRequestMessage request = new HttpRequestMessage(method.HttpMethod, url);
            if (method.Headers != null && method.Headers.Count > 0)
            {
                Util.AddRequestHeaders(request.Headers, method.Headers);
            }

            if (!string.IsNullOrEmpty(method.RequestData))
            {
                byte[] bytes = Encoding.GetBytes(method.RequestData);
                ByteArrayContent content = new ByteArrayContent(bytes);
                request.Content = content;
            }
            return request;
        }
        #endregion
        //private HttpWebResponse latestResponse;

        #region Constructors
        /// <summary>
        /// Every concrete class constructor should inherit from this constructor
        /// </summary>
        /// <param name="serverUrl"></param>
        public SessionBase(string serverUrl)
        {
            this.serverURL = serverUrl;
            
        }
        #endregion

        #region Public Properties
        public string ServerURL
        {
            get
            {
                return serverURL;
            }

            set
            {
                serverURL = value;
            }
        }

        /// <summary>
        /// A default headers set that will be used for every requests 
        /// Use Util.AddRequestHeaders() to add any header item to it.
        /// </summary>
        public HttpRequestHeaders MasterHeader
        {
            get
            {
                return this.HttpClient.DefaultRequestHeaders;
            }
            
        }

        public CookieContainer CookieContainer
        {
            get
            {
                return cookieContainer;
            }
        }

        public HttpClientHandler ClientHandler
        {
            get
            {
                if (clientHandler == null)
                {
                    clientHandler = new HttpClientHandler() { CookieContainer = this.CookieContainer };
                }
                return clientHandler;
            }
        }

        /// <summary>
        /// Encoding property will affect the way how RequestData will be generated to Byte array
        /// Encoding default is always UTF-8
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return encoding;
            }

            set
            {
                encoding = value;
            }
        }
        /// <summary>
        /// The last Response received from the server.
        /// It will be updated automatically in progress of sending requests.
        /// </summary>
        //public HttpWebResponse LatestResponse
        //{
        //    get
        //    {
        //        return latestResponse;
        //    }
        //}
        #endregion

        #region Public Methods
        public HttpResponseMessage Call(APIMethod method) 
        {

            HttpRequestMessage request = GetRequestMessage(method);
            HttpResponseMessage response = this.SendRequestRecursive(request);
            
            return response;
        }

        public Task<HttpResponseMessage> CallAsync(APIMethod method)
        {
            HttpRequestMessage request = GetRequestMessage(method);
            return HttpClient.SendAsync(request);
        }
        #endregion

        #region Protected members
        /// <summary>
        /// This method is used for sending any request to server. 
        /// In case the received response is a redirect response, it will automatically follow the server route.
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <returns>HttpResponseMessage of the request sent to the server. Check null before verifying the response.</returns>
        protected HttpResponseMessage SendRequestRecursive(HttpRequestMessage request)
        {
            //HttpClient client = new HttpClient() { MaxResponseContentBufferSize = 1000000 };
            //HttpRequestMessage request = new HttpRequestMessage(method.HttpMethod, url);
                      
            HttpResponseMessage returnResponse = null;
            try
            {
                returnResponse = HttpClient.SendAsync(request).Result;
            }
            catch (AggregateException exception)
            {
                //To-do: do something when an exception occurs
                Console.WriteLine("There is an excepion during sending the request: {0}", exception.Message);
            }
            
            //returnResponse = await client.SendAsync(request); 
           
            if (returnResponse != null)
            {
                
                //this.UpdateCookieCollection(returnResponse.);

                if (returnResponse.StatusCode == HttpStatusCode.Found || returnResponse.StatusCode == HttpStatusCode.Redirect ||
                    returnResponse.StatusCode == HttpStatusCode.RedirectMethod) //change method from POST => GET
                {
                    //returnResponse.Close();
                    HttpRequestMessage newRequest = new HttpRequestMessage();
                    //Add header for the request

                    //Don't Add request data since this is a GET request

                    newRequest.RequestUri = new Uri(returnResponse.Headers.GetValues("Location").FirstOrDefault());
                    newRequest.Method = HttpMethod.Get;
                    returnResponse = this.SendRequestRecursive(newRequest);
                }
                else if (returnResponse.StatusCode == HttpStatusCode.RedirectKeepVerb ||
                            returnResponse.StatusCode == HttpStatusCode.TemporaryRedirect)
                {
                    HttpRequestMessage newRequest = new HttpRequestMessage();
                    //To-do: Add header for the request

                    //Add request data
                    newRequest.Content = request.Content;

                    newRequest.RequestUri = new Uri(returnResponse.Headers.GetValues("Location").FirstOrDefault());
                    newRequest.Method = request.Method;
                    returnResponse = this.SendRequestRecursive(newRequest);
                }
            }
            else
            {
                Console.WriteLine("The response is null!");
            }
            
            return returnResponse;
        }

        

        /// <summary>
        /// Each instance of Session should have only one instance of HttpClient so Singleton and Lazy Initializing are used here
        /// </summary>
        protected HttpClient HttpClient
        {
            get
            {
                if (httpClient == null)
                {
                    httpClient = new HttpClient(this.ClientHandler);
                    httpClient.MaxResponseContentBufferSize = 1000000;
                }
                return httpClient;
            }
        }

       
        #endregion
    }
}
