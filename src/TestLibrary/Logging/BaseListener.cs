using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections;
using System.Net;
using NUnit.Framework.Interfaces;
//using NUnit.Core;
//using NUnit.Core.Extensibility;


namespace TestLibrary
{
    /// <summary>
    /// Base trace listener which does common formatting
    /// </summary>
    //[NUnitAddin(Description = "Event Timestamp Logger")]
    public class BaseListener : TextWriterTraceListener, IListener/*, IAddin, EventListener*/
    {
        public const string TestCaseDescriptionLine = "----------------------------------------------------------------------------------------------------------------------------------";
        public const int TitleLength = 20;
        protected const string NumberKey = "Number";
        protected const string FullNameKey = "FullName";
        protected const string DescriptionKey = "Description";
        protected const string TestNameKey = "TestName";
        protected const string ClassNameKey = "ClassName";
        protected const string CategoryKey = "Category";
        protected const string ResultKey = "Result";
        protected const string MessageKey = "Message";
        protected const string StacktraceKey = "StacktraceKey";
        #region Constructor & Destructor
        /// <summary>
        /// Base constructor which sets autoflush and header and footer of the trace file
        /// </summary>
        /// <param name="s">Stream</param>
        /// <param name="filter">LogFilter</param>
        /// <param name="header">Header</param>
        /// <param name="footer">Footer</param>
        public BaseListener(TextWriter writer, string header = null, string footer = null)
            : base(writer)
        {
            //this.Writer.AutoFlush = true;
            //logFilter = filter;
            if (!string.IsNullOrEmpty(header))
            {
                this.Writer.WriteLine(header);
            }
            if (!string.IsNullOrEmpty(footer))
            {
                this.footer = footer;
            }
            else
            {
                this.footer = string.Empty;
            }
            if (printedMethods != null && printedMethods.Count > 0)
            {
                printedMethods.Clear();
            }

            if (requests != null && requests.Count > 0)
            {
                requests.Clear();
            }

            if (responses != null && responses.Count > 0)
            {
                responses.Clear();
            }
        }

        ///// <summary>
        ///// Base constructor which sets autoflush and header and footer of the trace file
        ///// </summary>
        ///// <param name="s">Stream</param>
        ///// <param name="header">Header</param>
        ///// <param name="footer">Footer</param>
        ///// <param name="filter">LogFilter</param>
        //public BaseListener(Stream s, string header, string footer)
        //    : this(s, header, footer)
        //{
        //}

        /// <summary>
        /// Destructor: Closes the listener
        /// </summary>
        ~BaseListener()
        {
            this.Close(false);
        }
        #endregion

        #region Write & WriteLine
        public override void Write(string message)
        {
            this.Writer.Write(message);
        }
        public override void WriteLine(string message, string category)
        {
            if (category.Equals("testcase") && message.Equals("SetUp"))
            {
                PrintTestCaseDescription();
            }
            else if (category.Equals("testcase") && message.Equals("Teardown"))
            {
                PrintTestCaseResult();
            }
            else if (category.Equals("fixture") && message.Equals("Setup"))
            {
                PrintSetUpDescription();
            }
            else if (category.Equals("fixture") && message.Equals("Teardown"))
            {
                PrintTeardownDescription();
            }
            else if (category.Equals("fixture") && message.Equals("Sumary"))
            {
                PrintSumary();
            }
            else if (category.Equals("RequestData"))
            {
                PrintRequestData(message);
            }
            else 
            {
                Write(FormatMessage(message, category));
                Writer.WriteLine();
            }
        }

        public override void WriteLine(object message, string category)
        {
            if (category.Equals("Request"))
            {
                PrintRequest(message as HttpWebRequest);
            }
            else if (category.Equals("Response"))
            {
                PrintResponse(message as HttpWebResponse);
            }
        }

        public override void WriteLine(string message)
        {
            Writer.WriteLine(message);
            //WriteLine(message, LogLevel.Info.ToString());
        }

        public virtual string FormatMessage(string message, string category)
        {
            StringBuilder str = new StringBuilder();
            str.Append("[");
            str.Append(DateTime.Now.ToString(timeFormat));
            str.Append(" ");
            str.Append(string.Format("{0,-7}", category.ToUpper()));
            str.Append("] ");
            str.Append(new string(' ', Trace.IndentLevel * Trace.IndentSize));
            str.Append(message);
            return str.ToString();
        }


        #endregion

        #region Close
        ///// <summary>
        ///// Closes the writer and writes the footer if available
        ///// </summary>
        //public void Close()
        //{
        //    Close(true);
        //}

        public void Close(bool suppressGC)
        {
            try
            {
                // Write Footer
                if (!string.IsNullOrEmpty(footer))
                {
                    this.Writer.WriteLine(footer);
                }

                //WritePerformanceSummary();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            }

            // Close the TraceListener
            try
            {
                //this.Writer.Flush(); // not needed since autoflush is on
                //base.Flush();
                //base.Close();
                base.Dispose();
            }
            catch (Exception)
            {
            }

            if (suppressGC)
            {
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region Helper Method
        /// <summary>
        /// Based on the category and the already set LogFilter, should the message be shown.
        /// </summary>
        /// <param name="category">message's category</param>
        /// 
        /// <returns>true if LogFilter allows message's category to show</returns>
        //protected bool IsShowing(string category)
        //{
        //    return logFilter.IsOK(category.ToLogLevel());
        //}

        /// <summary>
        /// Write Performance Summary
        /// </summary>
        //protected virtual void WritePerformanceSummary()
        //{
        //    if (!string.IsNullOrEmpty(PerformanceSummaryString))
        //    {
        //        this.Writer.WriteLine(performanceSummaryString);
        //    }
        //}

        protected virtual void PrintRequest(HttpWebRequest request)
        {
            int requestId = request.GetHashCode();
            string url = request.RequestUri.ToString();
            string method = request.Method;
            string headerString = BuildHeadersString(request.Headers);

            Dictionary<string, string> requestInfo = new Dictionary<string, string>();
            requestInfo.Add("Url", url);
            requestInfo.Add("Method", method);
            requestInfo.Add("Headers", headerString);
            requestInfo.Add("Id", requestId.ToString());

            Requests[requestId] = requestInfo;

            PrintRequest(requestInfo);
        }

        protected virtual void PrintRequest(Dictionary<string, string> requestInfo)
        {
            WriteLine(string.Format("[{0}]: {1}", requestInfo["Method"], requestInfo["Url"]));
            WriteLine(string.Format("    Headers:"));
            string data = "        " + requestInfo["Headers"];
            data = data.Replace("\n", "\n        ");
            WriteLine(data);
        }

        protected static string BuildHeadersString(WebHeaderCollection headers)
        {
            StringBuilder headerString = new StringBuilder();

            //for (int i = 0; i < headers.Count; i++)
            //{
            //    String header = headers.GetKey(i);
            //    String[] values = headers.GetValues(header);
            //    headerString.Append(header + ":");
            //    if (values.Length > 0)
            //    {
            //        for (int j = 0; j < values.Length; j++)
            //        {
            //            headerString.Append(values[j] + ",");
            //        }
            //        headerString.Remove(headerString.Length - 1, 1);
            //    }
            //    headerString.Append("\n");
            //}
            foreach(string header in headers.AllKeys)
            {
                headerString.Append(header + ":" + headers[header]);
            }
            headerString.Append("\n");

            return headerString.ToString();
        }

        protected virtual void PrintResponse(HttpWebResponse response)
        {
            int responseId = response.GetHashCode();
            string data = Util.ReadTextFromHttpResponse(response);
            string statusCode = response.StatusCode.ToString();
            string headerString = BuildHeadersString(response.Headers);
            Dictionary<string, string> responseInfo = new Dictionary<string, string>();
            responseInfo.Add("HttpCode", statusCode);
            responseInfo.Add("Data", data);
            responseInfo.Add("Headers", headerString);
            responseInfo.Add("Id", responseId.ToString());

            Responses[responseId] = responseInfo;

            PrintResponse(responseInfo);
        }

        protected virtual void PrintResponse(Dictionary<string, string> responseInfo)
        {
            WriteLine("[RESPONSE]:");
            WriteLine(string.Format("    Http code: {0}", responseInfo["HttpCode"]));
            WriteLine(string.Format("    Response data:"));
            string data = "        " + responseInfo["Data"];
            string formatedString = data.Replace("\n", "\n        ");
            WriteLine(formatedString);
            WriteLine(string.Format("    Header:"));
            data = "        " + responseInfo["Headers"];
            formatedString = data.Replace("\n", "\n        ");
            WriteLine(formatedString);
        }

        protected virtual void PrintRequestData(string data)
        {
            WriteLine(string.Format("    Request Data:"));

            string formatedString = ("        " + data).Replace("\n", "\n        ");
            WriteLine(formatedString);
        }

        protected virtual void PrintTestCaseDescription()
        {
            TestContext context = TestContext.CurrentContext;
            //IDictionary temp = context.Test.Properties;
            StringBuilder categoryStr = new StringBuilder(string.Empty);
            string description = string.Empty;
            StringBuilder className = new StringBuilder(string.Empty);
            string testName = string.Empty;
            string methodFullName = context.Test.FullName;
            Dictionary<string, string> newMethod = null;
            if (context != null && context.Test != null)
            {
                newMethod = new Dictionary<string, string>();
                if (context.Test.Properties.ContainsKey("_CATEGORIES"))
                {
                    ArrayList categories = context.Test.Properties.Get("_CATEGORIES") as ArrayList;
                    if (categories != null && categories.Count > 0)
                    {
                        foreach (object item in categories)
                        {
                            categoryStr.Append(item).Append(",");
                        }
                        categoryStr.Remove(categoryStr.Length - 1, 1);
                    }
                } 
                if (context.Test.Properties.ContainsKey("_DESCRIPTION"))
                {
                    description = (string)context.Test.Properties.Get("_DESCRIPTION");
                }
                methodFullName = context.Test.FullName;
                string[] tempStr = methodFullName.Split('.');
                if (tempStr.Length > 1)
                {
                    for (int i = 0; i < tempStr.Length - 1; i++)
                    {
                        className.Append(tempStr[i]).Append('.');
                    }
                    className.Remove(className.Length - 1, 1);
                }
                testName = context.Test.Name;
                newMethod.Add(NumberKey, (PrintedMethods.Count).ToString());
                newMethod.Add(FullNameKey, methodFullName);
                newMethod.Add(DescriptionKey, description);
                newMethod.Add(CategoryKey, categoryStr.ToString());
                newMethod.Add(ClassNameKey, className.ToString());
                newMethod.Add(TestNameKey, testName);
                PrintedMethods[methodFullName] = newMethod;
            }
            PrintTestCaseDescription(PrintedMethods[methodFullName]);
        }

        protected virtual void PrintTestCaseDescription(Dictionary<string, string> newMethod)
        {
            WriteLine(TestCaseDescriptionLine);
            //IDictionary temp = context.Test.Properties;
            PrintTestCaseDescriptionLine("Test number", newMethod["Number"]);
            PrintTestCaseDescriptionLine("Category", newMethod["Category"]);
            PrintTestCaseDescriptionLine("Description", newMethod["Description"]);
            PrintTestCaseDescriptionLine("Test Class", newMethod["ClassName"]);
            PrintTestCaseDescriptionLine("Test method", newMethod["TestName"]);

        }

        protected virtual void PrintTestCaseResult()
        {
            TestContext context = TestContext.CurrentContext;
            //IDictionary temp = context.Test.Properties;
            //StringBuilder categoryStr = new StringBuilder(string.Empty);
            //string description = string.Empty;
            //StringBuilder className = new StringBuilder(string.Empty);
            //string testName = string.Empty;
            string methodFullName = context.Test.FullName;
            Dictionary<string, string> currentMethod = null;
            if (PrintedMethods.ContainsKey(methodFullName))
            {
                currentMethod = PrintedMethods[methodFullName];
                currentMethod[ResultKey] = context.Result.Outcome.Status.ToString();

                PrintTestCaseResult(context.Result.Outcome.Status.ToString());
                if (!string.IsNullOrEmpty(context.Result.Message))
                {
                    PrintTestCaseMessage(context.Result.Message);
                }
                if (!string.IsNullOrEmpty(context.Result.StackTrace))
                {
                    PrintTestCaseStackTrace(context.Result.StackTrace);
                }
            }
        }

        protected virtual void PrintTestCaseResult(string result)
        {
            WriteLine(string.Format("Test result: {0}", result));

        }

        protected virtual void PrintTestCaseMessage(string message)
        {
            WriteLine(string.Format("Message: {0}", message));
        }

        protected virtual void PrintTestCaseStackTrace(string stackTrace)
        {
            WriteLine(string.Format("Stack trace: {0}", stackTrace));
        }

        protected virtual void PrintSetUpDescription()
        {
            WriteLine(TestCaseDescriptionLine);
            TestContext context = TestContext.CurrentContext;

            //IDictionary temp = context.Test.Properties;
            string methodFullName = context.Test.FullName;
            string[] tempStr = methodFullName.Split('.');
            if (tempStr.Length > 1)
            {
                PrintTestCaseDescriptionLine("Test fixture", tempStr[0]);
            }
        }

        protected virtual void PrintTeardownDescription()
        {
            WriteLine(TestCaseDescriptionLine);
            TestContext context = TestContext.CurrentContext;

            //IDictionary temp = context.Test.Properties;
            string methodFullName = context.Test.FullName;
            string[] tempStr = methodFullName.Split('.');
            if (tempStr.Length > 1)
            {
                PrintTestCaseDescriptionLine("End Test fixture", tempStr[0]);
            }
        }

        protected virtual void PrintSumary()
        {
            IEnumerable<KeyValuePair<string, Dictionary<string, string>>> temp = PrintedMethods.AsQueryable();
            IEnumerable<Dictionary<string, string>> failedQuery =
                PrintedMethods.Where(x => !x.Value[ResultKey].Equals(TestStatus.Passed.ToString())).
                Select(x => x.Value);
            List<Dictionary<string, string>> failedList = failedQuery.ToList();
            int total = printedMethods.Count;
            int failed = failedList.Count;
            int inconclusive = PrintedMethods.Where(x => x.Value[ResultKey].Equals(TestStatus.Inconclusive.ToString())).
                Select(x => x.Value).Count();
            int skipped = PrintedMethods.Where(x => x.Value[ResultKey].Equals(TestStatus.Skipped.ToString())).
                Select(x => x.Value).Count();
            int passed = total - failed - skipped;
            PrintFailedTestList(failedList);
            PrintTestSumary(total, failed, inconclusive, skipped, passed);
        }

        protected virtual void PrintFailedTestList(List<Dictionary<string, string>> testList)
        {
            //WriteLine(TestCaseDescriptionLine);
            WriteLine("All failed tests");
            WriteLine(TestCaseDescriptionLine);
            foreach (Dictionary<string, string> testcase in testList)
                WriteLine(GetTableCell(testcase[FullNameKey], TestCaseDescriptionLine.Length - 1) + "|");
            WriteLine(TestCaseDescriptionLine);

        }

        protected virtual void PrintTestSumary(int total, int failed, int inconclusive, int skipped, int passed)
        {
            string title = string.Concat(GetTableCell("Total"), GetTableCell("Failed"),
                GetTableCell("Inconclusive"), GetTableCell("Skipped"), GetTableCell("Passed"));

            int maxLength = TestCaseDescriptionLine.Length - title.Length;

            for (int i = 0; i < maxLength; i++)
            {
                title += " ";
            }
            title += "|";

            string info = string.Concat(GetTableCell(total.ToString()), GetTableCell(failed.ToString()),
                GetTableCell(inconclusive.ToString()), GetTableCell(skipped.ToString()), GetTableCell(passed.ToString()));

            maxLength = TestCaseDescriptionLine.Length - info.Length;

            for (int i = 0; i < maxLength; i++)
            {
                info += " ";
            }
            info += "|";
            WriteLine("Sumary");
            WriteLine(TestCaseDescriptionLine);
            WriteLine(title);
            WriteLine(TestCaseDescriptionLine);
            WriteLine(info);
            WriteLine(TestCaseDescriptionLine);
        }

        protected string GetTableCell(string info, int maxLength = TitleLength)
        {
            string content = "|  " + info;
            int length = content.Length;
            for (int i = 0; i <= maxLength - length; i++)
            {
                content += ' ';
            }
            return content;
        }

        protected virtual void PrintTestCaseDescriptionLine(string title, string info)
        {
            //Log.Write("|   Test Class  | ");
            string result = "|  " + title;
            for (int i = 0; i < TitleLength - title.Length - 4; i++)
            {
                result += ' ';
            }
            result += "| ";
            string infoTemp = info == null ? string.Empty : info.Trim();
            result += infoTemp;
            int maxLength = TestCaseDescriptionLine.Length - TitleLength - 2;
            int count = maxLength - infoTemp.Length;
            for (int i = 0; i < count; i++)
            {
                result += " ";
            }
            result += "|";
            WriteLine(result);
            WriteLine(TestCaseDescriptionLine);
        }

        

        protected static Dictionary<string, Dictionary<string, string>> PrintedMethods
        {
            get
            {
                if (printedMethods == null)
                    printedMethods = new Dictionary<string, Dictionary<string, string>>();
                return printedMethods;
            }
        }

        protected static Dictionary<int, Dictionary<string, string>> Requests
        {
            get
            {
                if (requests == null)
                    requests = new Dictionary<int, Dictionary<string, string>>();
                return requests;
            }
        }

        protected static Dictionary<int, Dictionary<string, string>> Responses
        {
            get
            {
                if (responses == null)
                    responses = new Dictionary<int, Dictionary<string, string>>();

                return responses;
            }
        }
        #endregion

        #region Members
        /// <summary>
        /// TimeFormat of the timestamp
        /// </summary>
        protected string timeFormat = "HH:mm:ss";
        // protected string detailTimeFormat = "HH:mm:ss.ff"

        /// <summary>
        /// Value of Footer
        /// </summary>
        protected string footer;

        protected static Dictionary<string, Dictionary<string, string>> printedMethods;
        protected static Dictionary<int, Dictionary<string, string>> requests;
        protected static Dictionary<int, Dictionary<string, string>> responses;

        /// <summary>
        /// Value of Footer
        /// </summary>
        public virtual string Footer
        {
            get
            {
                return footer;
            }
            set
            {
                footer = value;
            }
        }
        #endregion
    }
}
