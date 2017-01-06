using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using NUnit.Framework;
using System.Collections;

namespace TestLibrary
{
    public class HtmlListener : BaseListener
    {
        #region Constructor
        ///// <summary>
        ///// Construct with no header and footer
        ///// </summary>
        ///// <param name="s"></param>
        //public HtmlListener(Stream s, string header = null, string footer = null)
        //    : this(s, header, footer)
        //{
        //}

        /// <summary>
        /// Construct with header and footer
        /// </summary>
        /// <param name="s"></param>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        public HtmlListener(string header = null, string footer = null) 
            : base(header,footer)
        {
            
        }
        #endregion

        /// <summary>
        /// Format the message into html format
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="category">log level category</param>
        /// <returns></returns>
        internal override string FormatMessage(CommonLog item)
        {
            //if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(category))
            //{
            //    throw new ArgumentException("Message or Category cannot be null");
            //    //return string.Empty;
            //}
            
            return string.Format("<li><b>{0}</b>{1} {2}</li> ", item.Type,item.Time, item.Name);
            
        }

        private void PrintHtmlStart(TextWriter writer)
        {
            writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"ja\" lang=\"ja\">");

            writer.WriteLine("<head>");
            writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset= utf-8\" />");
            writer.WriteLine("<meta name = \"language\" content = \"ja\" />");

            AddStyleSheet(writer,"testReportStyles.css");
            AddScript(writer, "testReportScript.js");

            writer.WriteLine("</head>"); // end Head

            writer.WriteLine("<body>");
            if (!string.IsNullOrEmpty(Header))
            {
                writer.WriteLine(string.Format("<h1 id={0}>{1}</h1>", TOP_ID, Header));
            }

            AddToggleButtons(writer);

            writer.WriteLine("<br />");
            writer.WriteLine("<div class=\"css-treeview\">");
            writer.WriteLine("<ul>");
        }

        internal override void PrintTestCaseDescription(TextWriter writer, TestMethodLog newMethod)
        {
            //StringBuilder str = new StringBuilder();
            if (isRequestUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }
            if (isTestCaseUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }
            
            string temp = "Testcase" + newMethod.Number;
            writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" title=\"{2}\" class=\"testcase\"><b>{1}</b></label>", 
                temp, newMethod.Number + "."+ newMethod.TestName, newMethod.Description));
            writer.WriteLine("<ul>");
            isTestCaseUlOpen = true;
            writer.WriteLine(string.Format("<li>{0}</li>", "<b>Category:</b>&nbsp;  " + newMethod.Category));
            writer.WriteLine(string.Format("<li>{0}</li>", "<b>Bug:</b>&nbsp;  " + newMethod.Bug));
            writer.WriteLine(string.Format("<li>{0}</li>", "<b>Description:</b>&nbsp;  " + newMethod.Description));
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
            //writer.Write(str);
        }

        internal override void PrintTestCaseResult(TextWriter writer, string result)
        {
            writer.WriteLine(string.Format("<li>{0}</li>", "<b>Result:</b>&nbsp;  " + result));
        }

        internal override void PrintTestCaseMessage(TextWriter writer, string message)
        {
            writer.WriteLine(string.Format("<li>{0}</li>", "<b>Message:</b>&nbsp;  " + message));
        }

        internal override void PrintTestCaseStackTrace(TextWriter writer, string stackTrace)
        {
            StringBuilder str = new StringBuilder();
            writer.WriteLine("<li><b>Stack trace:</b></li>");
            if (!string.IsNullOrEmpty(stackTrace))
            {
                writer.WriteLine(string.Format("<li><textarea readOnly ='true'>{0}</textarea></li>", stackTrace));
            }

            writer.WriteLine(str.ToString());
        }

        internal override void PrintHeader(TextWriter writer)
        {
            PrintHtmlStart(writer);
        }

        internal override void PrintFooter(TextWriter writer)
        {
            // Add pre-set toggle buttons
            AddToggleButtons(writer);

            // Reformate Footer with proper <br/> break newline
            ReplaceNewLine(ref this.footer);

            // End body
            footer = string.Format("{1}{0}{1}</body>{1}",
                (string.IsNullOrEmpty(footer)) ? string.Empty : footer, Environment.NewLine);
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }

        internal override void PrintSetUpDescription(TextWriter writer)
        {
            

            string methodFullName = testFixture.Name;
            string[] tempStr = methodFullName.Split('.');
            if (isRequestUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }
            if (isTestCaseUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }
            if (tempStr.Length > 0)
            {
                string temp = "TestFixture."+ tempStr[0];
                writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" class=\"fixture\">{1}</label>", 
                    temp, "<b>Test Project:</b>&nbsp;" + tempStr[0]));
                writer.WriteLine("<ul>");
            }
            
        }

        internal override void PrintTeardownDescription(TextWriter writer)
        {
            string methodFullName = testFixture.Name;
            string[] tempStr = methodFullName.Split('.');
            if (isRequestUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }
            if (isTestCaseUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }

            if (isFixtureUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isFixtureUlOpen = false;
            }
            if (tempStr.Length > 0)
            {
                string temp = "EndTestFixture." + tempStr[0];
                writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" class=\"fixture\">{1}</label>", 
                    temp, "<b>End test Project:</b>&nbsp;" + tempStr[0]));
                writer.WriteLine("<ul>");
                isFixtureUlOpen = true;
            }
        }

        internal override void PrintFixtureSetUp(TextWriter writer)
        {
            
            string fixtureName = testFixture.Name;
            if (isRequestUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }
            if (isTestCaseUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }
            if (isFixtureUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isFixtureUlOpen = false;
            }

            string temp = "TestFixture." + fixtureName;
            writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" checked=\"checked\" /><label for=\"{0}\" class=\"fixture\">{1}</label>",
                temp, "<b>Test Fixture:</b>&nbsp;" + temp));
            writer.WriteLine("<ul>");
            isFixtureUlOpen = true;

            if (testFixture.SetUp.Children != null && testFixture.SetUp.Children.Count > 0)
            {
                foreach (var child in testFixture.SetUp.Children)
                {
                    PrintChildrenLog(writer, child);
                }
            }
        }

        internal override void PrintFixtureTearDown(TextWriter writer)
        {
            if (testFixture.SetUp.Children != null && testFixture.SetUp.Children.Count > 0)
            {
                foreach (var child in testFixture.SetUp.Children)
                {
                    PrintChildrenLog(writer, child);
                }
            }
            PrintSumary(writer);
        }

        internal override void PrintTestSumary(TextWriter writer, int total, int failed, int inconclusive, int skipped, int passed)
        {
            writer.WriteLine("<div class='sumary'>");
            writer.WriteLine("<p><b>Sumary</b></p>");
            writer.WriteLine("<table>");
            writer.WriteLine("<tr>");
            writer.WriteLine("<td>Total</td>");
            writer.WriteLine("<td>Failed</td>");
            writer.WriteLine("<td>Inconclusive</td>");
            writer.WriteLine("<td>Skipped</td>");
            writer.WriteLine("<td>Passed</td>");
            writer.WriteLine("</tr>");
            writer.WriteLine("<tr>");
            writer.WriteLine(string.Format("<td>{0}</td>", total));
            writer.WriteLine(string.Format("<td>{0}</td>", failed));
            writer.WriteLine(string.Format("<td>{0}</td>", inconclusive));
            writer.WriteLine(string.Format("<td>{0}</td>", skipped));
            writer.WriteLine(string.Format("<td>{0}</td>", passed));
            writer.WriteLine("</tr>");
            writer.WriteLine("</table>");
            writer.WriteLine("</div>");
            
        }

        internal override void PrintFailedTestList(TextWriter writer, List<TestMethodLog> testList)
        {
            // Close previous tag in the body
            if (isRequestUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }

            if (isTestCaseUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }
            
            if (isFixtureUlOpen)
            {
                writer.WriteLine("</ul>");
                writer.WriteLine("</li>");
                isFixtureUlOpen = false;
            }

            writer.WriteLine("</ul>");
            writer.WriteLine("</div>");
            writer.WriteLine("<div class='testFailed'>");
            writer.WriteLine("<p><b>Fail test list</b></p>");
            writer.WriteLine("<ul>");
            foreach (TestMethodLog item in testList)
            {
                writer.WriteLine(string.Format("<li><a href=\"#\" onclick=\"GoToTestCase('{0}'); return false;\">{1}</a></li>", "Testcase" + item.Number, 
                    item.FullName));
            }
            writer.WriteLine("</ul>");
            writer.WriteLine("</div>");
            //base.PrintFailedTestList(testList);
        }

        internal override void PrintRequest(TextWriter writer, Request requestInfo)
        {
            if (requestInfo == null)
                return;
            string temp = "Request" + requestInfo.Id;

            if (isRequestUlOpen)
            {
                writer.Write("</ul></li>");
                isRequestUlOpen = false;
            }
            writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" class=\"request\"><b>[{1}]</b><a>&nbsp;{2}</a></label>", 
                temp, requestInfo.Method, requestInfo.Url));
            writer.WriteLine("<ul>");
            //writer.Write(string.Format("<li><b>Url: </b>{1}</li>", "Url" + temp, requestInfo["Url"]));
            //writer.Write(string.Format("<li><b>Method:</b> {1}</li>", "Method"+temp, requestInfo["Method"]));
            writer.WriteLine("<li><b>Header:</b></li>");
            if(!string.IsNullOrEmpty(requestInfo.Headers))
            {
                writer.WriteLine(string.Format("<li><textarea readOnly='true'>{0}</textarea></li>", requestInfo.Headers));
            }
            isRequestUlOpen = true;
            PrintRequestData(writer, requestInfo.Data);
            PrintResponse(writer, requestInfo.Response);
        }

        internal override void PrintRequestData(TextWriter writer, string data)
        {
            StringBuilder str = new StringBuilder();
            writer.WriteLine("<li><b>Request data:</b></li>");
            if(!string.IsNullOrEmpty(data))
            {
                writer.WriteLine(string.Format("<li><textarea readOnly ='true'>{0}</textarea></li>", data));
            }
            
            writer.WriteLine(str.ToString());
        }

        internal override void PrintResponse(TextWriter writer, Response responseInfo)
        {
            if (responseInfo == null)
                return;
            string temp = "Response" + responseInfo.Id;
            writer.WriteLine(
                string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" class=\"response\"><b>Responses:</b></label>", temp));
            writer.WriteLine("<ul>");
            writer.WriteLine(string.Format("<li><b>Http code:</b> {0}</li>", responseInfo.HttpCode));
            writer.WriteLine(string.Format("<li><b>Data:</b></li>"));
            if(!string.IsNullOrEmpty(responseInfo.Data))
            {
                writer.WriteLine(string.Format("<li><textarea readOnly ='true'>{0}</textarea></li>", responseInfo.Data));
            }
            writer.WriteLine("<li><b>Headers:</b></li>");
            if (!string.IsNullOrEmpty(responseInfo.Headers))
            {
                writer.WriteLine(string.Format("<li><textarea readOnly ='true'>{0}</textarea></li>", responseInfo.Headers));
            }
            writer.WriteLine("</ul></li>");
            if (isRequestUlOpen)
            {
                writer.WriteLine("</ul></li>");
                isRequestUlOpen = false;
            }
        }

       
        #region protected helper methods to write Html content
        /// <summary>
        /// Replace new line with html <!--<br/>--> 
        /// </summary>
        /// <param name="message"></param>
        protected void ReplaceNewLine(ref string message)
        {
            // replace new line with html <br/>
            if (!string.IsNullOrEmpty(message) && message.IndexOf('\r') > -1)
            {
                message = message.Replace(Environment.NewLine, htmlNewLine);
            }

        }

        /// <summary>
        /// Add css Stylesheet with href=source
        /// </summary>
        /// <param name="source">css stylesheet</param>
        protected void AddStyleSheet(TextWriter writer, string source)
        {
            writer.WriteLine(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", source));
        }
        /// <summary>
        /// Add Javascript from source
        /// </summary>
        /// <param name="source">javascript source</param>
        protected void AddScript(TextWriter writer, string source)
        {
            writer.WriteLine(string.Format("<script language=\"JavaScript\" src=\"{0}\"></script>", source));
        }

        /// <summary>
        /// Add preset Toggle buttons: Expanse all, Collapse all
        /// 1. Toggle Debug: Debug, Enter, Exit
        /// 2. Toggle Repro: Repro, Verify, Success
        /// 3. Toggle Warn
        /// 4. Toggle Time
        /// </summary>
        protected virtual void AddToggleButtons(TextWriter writer)
        {
            //writer.AddAttribute(HtmlTextWriterAttribute.Id, "Buttons");
            writer.WriteLine("<div id=\"Buttons\">");
            AddToggleButton(writer, "Expanse all", "Expanse");
            AddToggleButton(writer, "Collapse all", "Collapse");
            writer.WriteLine("</div>"); // end Div
        }
        /// <summary>
        /// Add Toggle button for 1 category
        /// </summary>
        /// <param name="name">name of toggle button</param>
        /// <param name="category">log level category</param>
        protected void AddToggleButton(TextWriter writer, string name, string category)
        {
            string onclick = string.Format("javascript:{0}All()", category);
            string id = string.Format("Toggle {0}", name);
            AddButton(writer, id, id, name, onclick);
        }
        /// <summary>
        /// Add Toggle button for multiple categories
        /// </summary>
        /// <param name="name">name of toggle button</param>
        /// <param name="categories">log level categories</param>
        protected void AddToggleButton(TextWriter writer, string name, string[] categories)
        {
            if (categories != null || categories.Length > 0)
            {
                StringBuilder onclick = new StringBuilder();
                onclick.Append("javascript:ToggleItems([");
                foreach (string category in categories)
                {
                    onclick.AppendFormat("'{0}',", category);
                }
                onclick.Remove(onclick.Length - 1, 1); // remove the last ,
                onclick.Append("])");

                string id = string.Format("Toggle {0}", name);
                AddButton(writer, id, id, name, onclick.ToString());
            }
        }

        /// <summary>
        /// Add Go To Button
        /// </summary>
        /// <param name="divId">div Id to go to</param>
        protected void AddGoToButton(TextWriter writer, string divId)
        {
            string id = string.Format("{0}Button", divId);
            string title = string.Format("Go To {0}", divId);
            string onclick = string.Format("location.href='#{0}'", divId);
            AddButton(writer, id, title, divId, onclick);
        }

        /// <summary>
        /// Add a button
        /// </summary>
        /// <param name="id">input id</param>
        /// <param name="title">input title (hint)</param>
        /// <param name="value">button label</param>
        /// <param name="onclick">what to do onclick</param>
        protected void AddButton(TextWriter writer, string id, string title, string value, string onclick)
        {
            writer.WriteLine(
                string.Format("<input type=\"button\" id=\"{0}\" title=\"{1}\" value=\"{2}\" onclick=\"{3}\" />", 
                id, title,value, onclick));
            //writer.WriteLine();
        }
                
        #endregion

        #region protected members
        protected string htmlNewLine = string.Format("<br />{0}", Environment.NewLine);
        protected bool isFixtureUlOpen = false;
        protected bool isRequestUlOpen = false;
        protected bool isTestCaseUlOpen = false;
        //protected Dictionary<string, List<PerformanceData>> summaryDictionary;
        //protected int summaryCount
        //{
        //    get
        //    {
        //        return summaryDictionary.Count;
        //    }
        //}
        protected const string TOP_ID = "Top";
        protected const string PERFORMANCE_SUMMARY_ID = "Performance Summary";

        #endregion
    }
}
