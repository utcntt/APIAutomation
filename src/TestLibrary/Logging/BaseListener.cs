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
using NUnit.Framework.Internal;
using System.Net.Http;
using System.Net.Http.Headers;
//using NUnit.Core;
//using NUnit.Core.Extensibility;


namespace TestLibrary
{
    /// <summary>
    /// Base trace listener which does common formatting
    /// </summary>
    //[NUnitAddin(Description = "Event Timestamp Logger")]
    public class BaseListener 
    {
        public const string TestCaseDescriptionLine = "----------------------------------------------------------------------------------------------------------------------------------";
        public const int TitleLength = 20;
        protected const string NumberKey = "Number";
        protected const string FullNameKey = "FullName";
        protected const string DescriptionKey = "Description";
        protected const string BugKey = "Bug";
        protected const string TestNameKey = "TestName";
        protected const string ClassNameKey = "ClassName";
        protected const string CategoryKey = "Category";
        protected const string ResultKey = "Result";
        protected const string MessageKey = "Message";
        protected const string StacktraceKey = "StacktraceKey";
        protected const string RequestKey = "Request";
        protected const string RequestDataKey = "RequestData";
        protected const string ResponseKey = "Response";
        
        #region Constructor & Destructor
        /// <summary>
        /// Base constructor which sets autoflush and header and footer of the trace file
        /// </summary>
        /// <param name="header">Header</param>
        /// <param name="footer">Footer</param>
        public BaseListener(string header = null, string footer = null)
            //: base(writer)
        {
            this.Header = header;
            this.footer = footer;
            testFixture = new TestFixture();
        }
        #endregion

        #region Public Methods
        public virtual void LogRequest(HttpRequestMessage request)
        {
            int requestId = request.GetHashCode();
            string url = request.RequestUri.ToString();
            string method = request.Method.Method;
            string headerString = BuildHeadersString(request.Headers);

            Request logRequest = new Request();
            logRequest.Url = url;
            logRequest.Method = method;
            logRequest.Headers = headerString;
            logRequest.Id = requestId.ToString();
            if (testFixture.CurrentNode is Request)
            {
                testFixture.CurrentNode.Parent.Children.Add(logRequest);
                logRequest.Parent = testFixture.CurrentNode.Parent;
                this.testFixture.CurrentNode = logRequest;
            }
            else
            {
                this.testFixture.CurrentNode.Children.Add(logRequest);
                logRequest.Parent = testFixture.CurrentNode;
                this.testFixture.CurrentNode = logRequest;
            }
        }

        public virtual void LogRequestData(string data)
        {
            if (testFixture.CurrentNode != null && (testFixture.CurrentNode is Request))
            {
                (testFixture.CurrentNode as Request).Data = data;
            }
                
        }

        public virtual void LogResponse(HttpResponseMessage response)
        {
            int responseId = response.GetHashCode();
            string data = Util.ReadTextFromHttpResponse(response).Result;
            string statusCode = response.StatusCode.ToString();
            string headerString = BuildHeadersString(response.Headers);
            if (testFixture.CurrentNode != null && (testFixture.CurrentNode is Request) 
                && ((Request)testFixture.CurrentNode).Response == null)
            {
                Response responseInfo = new Response();
                responseInfo.HttpCode = statusCode;
                responseInfo.Data = data;
                responseInfo.Headers = headerString;
                responseInfo.Id = responseId.ToString();

                (testFixture.CurrentNode as Request).Response = responseInfo;

            }
        }

        /// <summary>
        /// Testcase setup
        /// </summary>
        public virtual void LogTestCaseDescription()
        {
            TestContext context = TestContext.CurrentContext;
            //IDictionary temp = context.Test.Properties;
            StringBuilder categoryStr = new StringBuilder(string.Empty);
            string description = string.Empty;
            string className = string.Empty;
            string testName = string.Empty;
            string methodFullName = context.Test.FullName;
            string bug = string.Empty;
            TestMethodLog newMethod = null;
            if (context != null && context.Test != null)
            {
                newMethod = new TestMethodLog();
                if (context.Test.Properties.ContainsKey(PropertyNames.Category))
                {
                    ArrayList categories = context.Test.Properties.Get(PropertyNames.Category) as ArrayList;
                    if (categories != null && categories.Count > 0)
                    {
                        foreach (object item in categories)
                        {
                            categoryStr.Append(item).Append(",");
                        }
                        categoryStr.Remove(categoryStr.Length - 1, 1);
                    }
                    else
                    {
                        categoryStr.Append(context.Test.Properties.Get(PropertyNames.Category) as string);
                    }
                }
                if (context.Test.Properties.ContainsKey(PropertyNames.Description))
                {
                    description = (string)context.Test.Properties.Get(PropertyNames.Description);
                }
                if (context.Test.Properties.ContainsKey(BugAttribute.PropertyName))
                {
                    bug = (string)context.Test.Properties.Get(BugAttribute.PropertyName);
                }

                methodFullName = context.Test.FullName;

                className = context.Test.ClassName;
                testName = context.Test.Name;
                newMethod.Number = context.Test.ID;
                newMethod.FullName = methodFullName;
                newMethod.Description = description;
                newMethod.Category = categoryStr.ToString();
                newMethod.ClassName = className.ToString();
                newMethod.TestName =  testName;
                newMethod.Bug = bug;
                this.testFixture.Children.Add(newMethod);
                newMethod.Parent = testFixture;
                testFixture.CurrentNode = newMethod;
            }
            //PrintTestCaseDescription(PrintedMethods[methodFullName]);
        }

        public virtual void LogTestCaseResult()
        {
            TestContext context = TestContext.CurrentContext;
            string methodFullName = context.Test.FullName;
            TestMethodLog currentMethod = null;
            if (testFixture.CurrentNode != null && testFixture.CurrentNode is TestMethodLog)
            {
                currentMethod = testFixture.CurrentNode as TestMethodLog;
            }
            else if(testFixture.CurrentNode != null && (testFixture.CurrentNode is Request))
            {
                if (testFixture.CurrentNode.Parent is TestMethodLog)
                    currentMethod = testFixture.CurrentNode.Parent as TestMethodLog;
            }
            if (currentMethod != null)
            {
                currentMethod.Result = context.Result.Outcome.Status.ToString();

                //PrintTestCaseResult(context.Result.Outcome.Status.ToString());
                if (!string.IsNullOrEmpty(context.Result.Message))
                {
                    currentMethod.Message = context.Result.Message;
                }
                if (!string.IsNullOrEmpty(context.Result.StackTrace))
                {
                    currentMethod.Stacktrace = context.Result.StackTrace;
                }
                testFixture.CurrentNode = currentMethod;
            }
        }

        /// <summary>
        /// Fixture setup
        /// </summary>
        public virtual void LogSetUpDescription()
        {
            TestContext context = TestContext.CurrentContext;
            string methodFullName = context.Test.FullName;
            testFixture.Name = context.Test.ClassName;
            this.testFixture.SetUp.Name = methodFullName;
            this.testFixture.CurrentNode = testFixture.SetUp;
            
        }

        /// <summary>
        /// Fixture teardown
        /// </summary>
        public virtual void LogTeardownDescription()
        {
            TestContext context = TestContext.CurrentContext;
            string methodFullName = context.Test.FullName;
            testFixture.TearDown = new SetUpTearDown() { Name = methodFullName, Parent = testFixture };
            testFixture.CurrentNode = testFixture.TearDown;

        }

        public void WriteToOutput(TextWriter writer, Log.ParamLogLevel logLevel = Log.ParamLogLevel.Fail)
        {
            PrintHeader(writer);
            PrintFixtureSetUp(writer);
            foreach(var item in testFixture.Children)
            {
                if(item is TestMethodLog )
                {
                    TestMethodLog method = (TestMethodLog)item;
                    if(logLevel == Log.ParamLogLevel.Fail)
                    {
                        if (method.Result == TestStatus.Inconclusive.ToString() || method.Result == TestStatus.Failed.ToString())
                        {
                            PrintChildrenLog(writer, item);
                        }
                    }
                    else if(logLevel == Log.ParamLogLevel.Full)
                    {
                        PrintChildrenLog(writer, item);
                    }
                    
                }
            }
            PrintFixtureTearDown(writer);
            PrintSumary(writer);
            PrintFooter(writer);
        }
        #endregion

        #region Write & WriteLine
        //public override void Write(string message)
        //{
        //    this.Writer.Write(message);
        //}
        public void WriteLine(string message, string category)
        {
            if(testFixture.CurrentNode != null)
            {
                CommonLog commonLog = new TestLibrary.CommonLog();
                commonLog.Type = category;
                commonLog.Time = DateTime.Now.ToString();
                commonLog.Name = message;
                testFixture.CurrentNode.Children.Add(commonLog);
                commonLog.Parent = testFixture.CurrentNode;
            }
            
        }

        //public override void WriteLine(object message, string category)
        //{
        //    if (category.Equals("Request"))
        //    {
        //        PrintRequest(message as HttpWebRequest);
        //    }
        //    else if (category.Equals("Response"))
        //    {
        //        PrintResponse(message as HttpWebResponse);
        //    }
        //}

        protected void WriteLine(TextWriter writer, string message)
        {
            writer.WriteLine(message);
            //WriteLine(message, LogLevel.Info.ToString());
        }

        internal virtual void PrintHeader(TextWriter writer)
        {

        }

        internal virtual void PrintFooter(TextWriter writer)
        {

        }

        internal virtual string FormatMessage(CommonLog item)
        {
            StringBuilder str = new StringBuilder();
            str.Append("[");
            str.Append(item.Time);
            str.Append(" ");
            str.Append(string.Format("{0,-7}", item.Type.ToUpper()));
            str.Append("] ");
            str.Append(new string(' ', Trace.IndentLevel * Trace.IndentSize));
            str.Append(item.Name);
            return str.ToString();
        }


        #endregion

        #region Protected Methods
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

        internal virtual void PrintRequest(TextWriter writer, Request requestInfo)
        {
            WriteLine(writer, string.Format("[{0}]: {1}", requestInfo.Method, requestInfo.Url));
            WriteLine(writer, string.Format("    Headers:"));
            string data = "        " + requestInfo.Headers;
            data = data.Replace("\n", "\n        ");
            WriteLine(writer, data);

            PrintRequestData(writer, requestInfo.Data);
            if (requestInfo.Children != null && requestInfo.Children.Count > 0)
            {
                foreach (var child in requestInfo.Children)
                {
                    PrintChildrenLog(writer, child);
                }
            }
        }

        protected internal static string BuildHeadersString(HttpRequestHeaders headers)
        {
            StringBuilder headerString = new StringBuilder();

            foreach(var header in headers)
            {
                headerString.Append(header.Key + ":" + header.Value);
                headerString.Append("\n");
            }
            return headerString.ToString();
        }

        protected internal static string BuildHeadersString(HttpResponseHeaders headers)
        {
            StringBuilder headerString = new StringBuilder();

            foreach (var header in headers)
            {
                headerString.Append(header.Key + ":" + header.Value);
                headerString.Append("\n");
            }
            return headerString.ToString();
        }


        internal virtual void PrintResponse(TextWriter writer, Response responseInfo)
        {
            WriteLine(writer, "[RESPONSE]:");
            WriteLine(writer, string.Format("    Http code: {0}", responseInfo.HttpCode));
            WriteLine(writer, string.Format("    Response data:"));
            string data = "        " + responseInfo.Data;
            string formatedString = data.Replace("\n", "\n        ");
            WriteLine(writer, formatedString);
            WriteLine(writer, string.Format("    Header:"));
            data = "        " + responseInfo.Headers;
            formatedString = data.Replace("\n", "\n        ");
            WriteLine(writer, formatedString);
        }

        internal virtual void PrintRequestData(TextWriter writer, string data)
        {
            WriteLine(writer, string.Format("    Request Data:"));

            string formatedString = ("        " + data).Replace("\n", "\n        ");
            WriteLine(writer, formatedString);
        }



        internal virtual void PrintTestCaseDescription(TextWriter writer, TestMethodLog newMethod)
        {
            WriteLine(writer, TestCaseDescriptionLine);
            //IDictionary temp = context.Test.Properties;
            PrintTestCaseDescriptionLine(writer, "Test number", newMethod.Number);
            PrintTestCaseDescriptionLine(writer, "Category", newMethod.Category);
            PrintTestCaseDescriptionLine(writer, "Bug", newMethod.Bug);
            PrintTestCaseDescriptionLine(writer, "Description", newMethod.Description);
            PrintTestCaseDescriptionLine(writer, "Test Class", newMethod.ClassName);
            PrintTestCaseDescriptionLine(writer, "Test method", newMethod.TestName);

            if (newMethod.Children != null && newMethod.Children.Count > 0)
            {
                foreach (var child in newMethod.Children)
                {
                    PrintChildrenLog(writer, child);
                }
            }
            this.PrintTestCaseResult(writer, newMethod.Result);
            if (!string.IsNullOrEmpty(newMethod.Message))
            {
                this.PrintTestCaseMessage(writer, newMethod.Message);
            }

            if (!string.IsNullOrEmpty(newMethod.Stacktrace))
            {
                this.PrintTestCaseStackTrace(writer, newMethod.Stacktrace);
            }
        }

        internal void PrintChildrenLog(TextWriter writer, LogModel item)
        {
            if (item is Request)
            {
                PrintRequest(writer, (Request)item);
            }
            else if(item is TestMethodLog)
            {
                PrintTestCaseDescription(writer, (TestMethodLog)item);
            }
            else if (item is CommonLog)
            {
                WriteLine(writer, FormatMessage((CommonLog)item));
            }
        }

        internal virtual void PrintTestCaseResult(TextWriter writer, string result)
        {
            WriteLine(writer, string.Format("Test result: {0}", result));

        }

        internal virtual void PrintTestCaseMessage(TextWriter writer, string message)
        {
            WriteLine(writer, string.Format("Message: {0}", message));
        }

        internal virtual void PrintTestCaseStackTrace(TextWriter writer, string stackTrace)
        {
            WriteLine(writer, string.Format("Stack trace: {0}", stackTrace));
        }

        internal virtual void PrintSumary(TextWriter writer)
        {
            //IEnumerable<KeyValuePair<string, Dictionary<string, string>>> temp = PrintedMethods.AsQueryable();
            //IEnumerable<Dictionary<string, string>> failedQuery =
            //    PrintedMethods.Where(x => !x.Value[ResultKey].Equals(TestStatus.Passed.ToString())).
            //    Select(x => x.Value);
            var failedQuery = from t in testFixture.Children
                              where (t is TestMethodLog)
                              let method = (TestMethodLog)t
                              where (method.Result != null && method.Result == TestStatus.Failed.ToString())
                              select method;
            List<TestMethodLog> failedList = failedQuery.ToList();
            int total = testFixture.Children.Count;
            int failed = failedList.Count;
            int inconclusive = testFixture.Children
                .Where(x => (x is TestMethodLog && ((TestMethodLog)x).Result.Equals(TestStatus.Inconclusive.ToString())))
                .Select(x => x)
                .Count();
            int skipped = testFixture.Children
                .Where(x => (x is TestMethodLog && ((TestMethodLog)x).Result.Equals(TestStatus.Skipped.ToString())))
                .Select(x => x).Count();
            int passed = total - failed - skipped - inconclusive;
            PrintFailedTestList(writer, failedList);
            PrintTestSumary(writer, total, failed, inconclusive, skipped, passed);
        }

        internal virtual void PrintSetUpDescription(TextWriter writer)
        {
            WriteLine(writer, TestCaseDescriptionLine);
            //IDictionary temp = context.Test.Properties;
            string methodFullName = testFixture.SetUp.Name;
            string[] tempStr = methodFullName.Split('.');
            if (tempStr.Length > 1)
            {
                PrintTestCaseDescriptionLine(writer, "Test project", tempStr[0]);
            }
            
        }

        internal virtual void PrintTeardownDescription(TextWriter writer)
        {
            WriteLine(writer, TestCaseDescriptionLine);
            
            string methodFullName = testFixture.Name;
            string[] tempStr = methodFullName.Split('.');
            if (tempStr.Length > 1)
            {
                PrintTestCaseDescriptionLine(writer, "End Test fixture", tempStr[0]);
            }
            
        }


        internal virtual void PrintFailedTestList(TextWriter writer, List<TestMethodLog> testList)
        {
            //WriteLine(TestCaseDescriptionLine);
            WriteLine(writer, "All failed tests");
            WriteLine(writer, TestCaseDescriptionLine);
            foreach (TestMethodLog testcase in testList)
                WriteLine(writer, GetTableCell(testcase.Number , TestCaseDescriptionLine.Length - 1) + "|");
            WriteLine(writer, TestCaseDescriptionLine);

        }

        internal virtual void PrintTestSumary(TextWriter writer, int total, int failed, int inconclusive, int skipped, int passed)
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
            WriteLine(writer, "Sumary");
            WriteLine(writer, TestCaseDescriptionLine);
            WriteLine(writer, title);
            WriteLine(writer, TestCaseDescriptionLine);
            WriteLine(writer, info);
            WriteLine(writer, TestCaseDescriptionLine);
        }

        internal virtual void PrintFixtureSetUp(TextWriter writer)
        {
            WriteLine(writer, TestCaseDescriptionLine);
            
            PrintTestCaseDescriptionLine(writer,"Test fixture", testFixture.Name);
            if (testFixture.SetUp.Children != null && testFixture.SetUp.Children.Count > 0)
            {
                foreach (var child in testFixture.SetUp.Children)
                {
                    PrintChildrenLog(writer, child);
                }
            }
        }

        internal virtual void PrintFixtureTearDown(TextWriter writer)
        {
            WriteLine(writer, TestCaseDescriptionLine);
            PrintTestCaseDescriptionLine(writer, "Test fixture", testFixture.Name);
            if (testFixture.TearDown.Children != null && testFixture.TearDown.Children.Count > 0)
            {
                foreach (var child in testFixture.TearDown.Children)
                {
                    PrintChildrenLog(writer, child);
                }
            }
        }

        internal string GetTableCell(string info, int maxLength = TitleLength)
        {
            string content = "|  " + info;
            int length = content.Length;
            for (int i = 0; i <= maxLength - length; i++)
            {
                content += ' ';
            }
            return content;
        }

        internal virtual void PrintTestCaseDescriptionLine(TextWriter writer, string title, string info)
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
            WriteLine(writer, result);
            WriteLine(writer, TestCaseDescriptionLine);
        }

        protected static Dictionary<string, object> setUpMethod;
        protected static Dictionary<string, object> tearDownMethod;
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
        
        internal TestFixture testFixture;

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

        public virtual string Header { get; set; }
        #endregion

        
    }
    #region LogModel
    internal class LogModel
    {
        public string Type;
        public string Name;
        public LogTree Parent;
    }

    internal class LogTree: LogModel
    {
        internal List<LogModel> Children;

        internal LogTree()
        {
            Children = new List<LogModel>();
        }
    }

    internal class TestFixture : LogTree
    {
        internal SetUpTearDown SetUp = new SetUpTearDown();
        internal SetUpTearDown TearDown;
        internal LogTree CurrentNode;

        internal TestFixture() : base()
        {
            Type = "TestFixture";
            CurrentNode = SetUp;
            SetUp.Parent = this;
        }
    }

    internal class TestMethodLog : LogTree
    {
        internal TestMethodLog() : base()
        {
            Type = "TestMethod";
        }

        internal SetUpTearDown SetUp;
        internal SetUpTearDown TearDown;

        internal string Number;
        internal string FullName;
        internal string Description;
        internal string Bug;
        internal string TestName;
        internal string ClassName;
        internal string Category;
        internal string Result;
        internal string Message;
        internal string Stacktrace;
    }

    internal class Request : LogTree
    {
        internal string Url;
        internal string Method;
        internal string Headers;
        internal string Id;
        internal string Data;
        internal Response Response;
    }

    internal class SetUpTearDown : LogTree
    {
        internal SetUpTearDown() : base()
        {
            Type = "SetUpTearDown";
        }
    }

    internal class Response: LogModel
    {
        internal string HttpCode;
        internal string Data;
        internal string Headers;
        internal string Id;
    }

    internal class CommonLog: LogModel
    {
        internal string Time;
    }
    #endregion

}
