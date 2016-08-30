using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace APICore
{
    public class Util
    {
        /// <summary>
        /// This method is used to add header information to a request. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        public static void AddRequestHeaders(HttpRequestHeaders requestHeaders, Dictionary<string, string> headers)
        {
            if (headers != null && headers.Count >= 1)
            {
                foreach (KeyValuePair<string, string> headerItem in headers)
                {
                    bool isAdded = requestHeaders.TryAddWithoutValidation(headerItem.Key, headerItem.Value);
                    //To-do: Warning in case isAdded = false;
                    if (!isAdded)
                    {
                        AddRequestHeaderItem(requestHeaders, headerItem);
                    }
                }
            }
        }

        /// <summary>
        /// This method is used for trying to add a header to a request in case TryAddWithoutValidation() is not successful.
        /// It is still under construction.
        /// </summary>
        /// <param name="requestHeaders"></param>
        /// <param name="headerItem"></param>
        public static void AddRequestHeaderItem(HttpRequestHeaders requestHeaders, KeyValuePair<string, string> headerItem)
        {
            switch (headerItem.Key.ToLower())
            {
                
            }
        }
        /// <summary>
        /// Generated the URL based on the parameters set at urlParameters
        /// </summary>
        /// <param name="urlParameters">Contains all the Key=>Values parameters that should be included in the url</param>
        /// <returns></returns>
        public static string FormatURLParameters(Dictionary<string, object> urlParameters)
        {
            // Verify URL Parameters are set
            if (null == urlParameters || urlParameters.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder formattedUrl = new StringBuilder("?");
            Type valueType = null;
            foreach (KeyValuePair<string, object> item in urlParameters)
            {
                valueType = item.Value != null ? item.Value.GetType() : null;
                if (valueType != null && valueType.IsGenericParameter && valueType.FullName.Contains("List"))
                {
                    IEnumerable parameter = item.Value as IEnumerable;
                    foreach (object para in parameter)
                    {
                        Util.AddQueryParam(formattedUrl, item.Key, para == null ? "" : para.ToString());
                    }
                }
                else
                {
                    Util.AddQueryParam(formattedUrl, item.Key, item.Value == null ? "" : item.Value.ToString());
                }
            }
            // Removing last "&"
            return formattedUrl.ToString();
        }

        //private static List<string> printedMethods;
        /// <summary>
        /// Add a key-value pair to a http query string. 
        /// Use this method in case you want to build an URL.
        /// </summary>
        /// <param name="source">URL</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="isAutoAddDelim">Delim "?" will be added automatically.</param>
        /// <returns></returns>
        public static StringBuilder AddQueryParam(StringBuilder source, string key, string value, bool isAutoAddDelim = false)
        {
            bool hasQuery = false;
            int length = source.Length;
            if (isAutoAddDelim)
            {
                for (int i = 0; i < length; i++)
                {
                    if (source[i] == '?')
                    {
                        hasQuery = true;
                        break;
                    }
                }
            }

            string delim = "&";
            if (!hasQuery && isAutoAddDelim)
            {
                delim = "?";
            }
            else if (((length > 0) && ((source[length - 1] == '?') || (source[length - 1] == '&'))) || length == 0)
            {
                delim = string.Empty;
            }

            string lowerKey = WebUtility.UrlEncode(key);
            string lowerValue = WebUtility.UrlEncode(value);
            Regex reg = new Regex(@"%[a-f0-9]{2}");
            string upperKey = reg.Replace(lowerKey, m => m.Value.ToUpperInvariant());
            string upperValue = reg.Replace(lowerValue, m => m.Value.ToUpperInvariant());
            return source.Append(delim).Append(upperKey).Append("=").Append(upperValue);
            //return source.Append(delim).Append(HttpUtility.UrlEncode(key))
            //  .Append("=").Append(HttpUtility.UrlEncode(value));
        }

        /// <summary>
        ///  Build a http query string WITHOUT "?" in beginning by using a dictionary of key and value. 
        ///  Use this method in case you want to build an query form data.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="paramList">Dictionary of key and value</param>
        /// <param name="isAutoAddDelim">Automatically add a "?" to the result</param>
        /// <returns>StringBuilder</returns>
        public static StringBuilder AddQueryParam(StringBuilder source, Dictionary<string, string> paramList, bool isAutoAddDelim = false)
        {
            foreach (KeyValuePair<string, string> item in paramList)
            {
                Util.AddQueryParam(source, item.Key, item.Value, isAutoAddDelim);
            }
            return source;
        }
    }
}
