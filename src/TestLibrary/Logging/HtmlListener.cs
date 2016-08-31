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
        public HtmlListener(TextWriter writer, string header, string footer)
            : base(writer)
        {
            this.Writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"ja\" lang=\"ja\">");

            this.Writer.WriteLine("<head>");
            this.Writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset= utf-8\" />");
            this.Writer.WriteLine("<meta name = \"language\" content = \"ja\" />");


            //this.Writer.WriteLine();


            AddStyleSheet("testReportStyles.css");
            AddScript("testReportScript.js");

            this.Writer.WriteLine("</head>"); // end Head

            this.Writer.WriteLine("<body>");
            if (!string.IsNullOrEmpty(header))
            {
                this.Writer.WriteLine(string.Format("<h1 id={0}>{1}</h1>", TOP_ID, header));
            }

            AddToggleButtons();

            this.Writer.WriteLine("<br />");
            this.Writer.WriteLine("<div class=\"css-treeview\">");
            this.Writer.WriteLine("<ul>");
        }
        #endregion

        /// <summary>
        /// Format the message into html format
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="category">log level category</param>
        /// <returns></returns>
        public override string FormatMessage(string message, string category)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(category))
            {
                throw new ArgumentException("Message or Category cannot be null");
                //return string.Empty;
            }
            StringBuilder str = new StringBuilder();
            this.Writer.WriteLine(string.Format("<li>{0}</li>", "<b>" + category + "</b>  " + DateTime.Now.ToString(timeFormat) + " " + message));
            return str.ToString();
        }

        protected override void PrintTestCaseDescription(Dictionary<string, string> newMethod)
        {
            //StringBuilder str = new StringBuilder();
            if (isRequestUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }
            if (isTestCaseUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }

            string temp = "Testcase" + newMethod["Number"];
            this.Writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" title=\"{2}\" class=\"testcase\"><b>{1}</b></label>",
                temp, newMethod["Number"] + "." + newMethod["FullName"], newMethod["Description"]));
            this.Writer.WriteLine("<ul>");
            isTestCaseUlOpen = true;
            this.Writer.WriteLine(string.Format("<li>{0}</li>", "<b>Category:</b>&nbsp;  " + newMethod["Category"]));
            this.Writer.WriteLine(string.Format("<li>{0}</li>", "<b>Description:</b>&nbsp;  " + newMethod["Description"]));
            //this.Writer.Write(str);
        }

        protected override void PrintTestCaseResult(string result)
        {
            this.Writer.WriteLine(string.Format("<li>{0}</li>", "<b>Result:</b>&nbsp;  " + result));
        }

        protected override void PrintTestCaseMessage(string message)
        {
            this.Writer.WriteLine(string.Format("<li>{0}</li>", "<b>Message:</b>&nbsp;  " + message));
        }

        protected override void PrintTestCaseStackTrace(string stackTrace)
        {
            StringBuilder str = new StringBuilder();
            this.Writer.WriteLine("<li><b>Stack trace:</b></li>");
            if (!string.IsNullOrEmpty(stackTrace))
            {
                this.Writer.WriteLine(string.Format("<li><textarea readOnly ='true'>{0}</textarea></li>", stackTrace));
            }

            this.Writer.WriteLine(str.ToString());
        }

        protected override void PrintSetUpDescription()
        {
            TestContext context = TestContext.CurrentContext;
            //IDictionary temp = context.Test.Properties;
            string methodFullName = context.Test.FullName;
            string[] tempStr = methodFullName.Split('.');
            if (isRequestUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }
            if (isTestCaseUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }
            if (tempStr.Length > 0)
            {
                string temp = "TestFixture." + tempStr[0];
                this.Writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" class=\"fixture\">{1}</label>",
                    temp, "<b>Test Project:</b>&nbsp;" + tempStr[0]));
                this.Writer.WriteLine("<ul>");
            }
        }

        protected override void PrintTeardownDescription()
        {
            TestContext context = TestContext.CurrentContext;
            //IDictionary temp = context.Test.Properties;
            string methodFullName = context.Test.FullName;
            string[] tempStr = methodFullName.Split('.');
            if (isRequestUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }
            if (isTestCaseUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }

            if (isFixtureUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isFixtureUlOpen = false;
            }
            if (tempStr.Length > 0)
            {
                string temp = "EndTestFixture." + tempStr[0];
                this.Writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" class=\"fixture\">{1}</label>",
                    temp, "<b>End test Project:</b>&nbsp;" + tempStr[0]));
                this.Writer.WriteLine("<ul>");
                isFixtureUlOpen = true;
            }
        }

        protected override void PrintFixtureSetUp()
        {
            TestContext context = TestContext.CurrentContext;
            //IDictionary temp = context.Test.Properties;
            string fixtureName = context.Test.FullName;
            if (isRequestUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }
            if (isTestCaseUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }
            if (isFixtureUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isFixtureUlOpen = false;
            }

            string temp = "TestFixture." + fixtureName;
            this.Writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" checked=\"checked\" /><label for=\"{0}\" class=\"fixture\">{1}</label>",
                temp, "<b>Test Fixture:</b>&nbsp;" + temp));
            this.Writer.WriteLine("<ul>");
            isFixtureUlOpen = true;
        }

        protected override void PrintFixtureTearDown()
        {
            //base.PrintFixtureTearDown();
            //PrintSumary();
        }

        protected override void PrintTestSumary(int total, int failed, int inconclusive, int skipped, int passed)
        {
            this.Writer.WriteLine("<div class='sumary'>");
            this.Writer.WriteLine("<p><b>Sumary</b></p>");
            this.Writer.WriteLine("<table>");
            this.Writer.WriteLine("<tr>");
            this.Writer.WriteLine("<td>Total</td>");
            this.Writer.WriteLine("<td>Failed</td>");
            this.Writer.WriteLine("<td>Inconclusive</td>");
            this.Writer.WriteLine("<td>Skipped</td>");
            this.Writer.WriteLine("<td>Passed</td>");
            this.Writer.WriteLine("</tr>");
            this.Writer.WriteLine("<tr>");
            this.Writer.WriteLine(string.Format("<td>{0}</td>", total));
            this.Writer.WriteLine(string.Format("<td>{0}</td>", failed));
            this.Writer.WriteLine(string.Format("<td>{0}</td>", inconclusive));
            this.Writer.WriteLine(string.Format("<td>{0}</td>", skipped));
            this.Writer.WriteLine(string.Format("<td>{0}</td>", passed));
            this.Writer.WriteLine("</tr>");
            this.Writer.WriteLine("</table>");
            this.Writer.WriteLine("</div>");
            // Add pre-set toggle buttons
            AddToggleButtons();

            // Reformate Footer with proper <br/> break newline
            ReplaceNewLine(ref this.footer);

            // End body
            footer = string.Format("{1}{0}{1}</body>{1}",
                (string.IsNullOrEmpty(footer)) ? string.Empty : footer, Environment.NewLine);
            this.Writer.WriteLine("</body>");
            this.Writer.WriteLine("</html>");
        }

        protected override void PrintFailedTestList(List<Dictionary<string, string>> testList)
        {
            // Close previous tag in the body
            if (isRequestUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isRequestUlOpen = false;
            }

            if (isTestCaseUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isTestCaseUlOpen = false;
            }

            if (isFixtureUlOpen)
            {
                this.Writer.WriteLine("</ul>");
                this.Writer.WriteLine("</li>");
                isFixtureUlOpen = false;
            }

            this.Writer.WriteLine("</ul>");
            this.Writer.WriteLine("</div>");
            this.Writer.WriteLine("<div class='testFailed'>");
            this.Writer.WriteLine("<p><b>Fail test list</b></p>");
            this.Writer.WriteLine("<ul>");
            foreach (Dictionary<string, string> item in testList)
            {
                this.Writer.WriteLine(string.Format("<li><a href=\"#\" onclick=\"GoToTestCase('{0}'); return false;\">{1}</a></li>", "Testcase" + item["Number"],
                    item[BaseListener.FullNameKey]));
            }
            this.Writer.WriteLine("</ul>");
            this.Writer.WriteLine("</div>");
            //base.PrintFailedTestList(testList);
        }

        protected override void PrintRequest(Dictionary<string, string> requestInfo)
        {
            string temp = "Request" + requestInfo["Id"];

            if (isRequestUlOpen)
            {
                this.Writer.Write("</ul></li>");
                isRequestUlOpen = false;
            }
            this.Writer.WriteLine(string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" class=\"request\"><b>[{1}]</b><a>&nbsp;{2}</a></label>",
                temp, requestInfo["Method"], requestInfo["Url"]));
            this.Writer.WriteLine("<ul>");
            //this.Writer.Write(string.Format("<li><b>Url: </b>{1}</li>", "Url" + temp, requestInfo["Url"]));
            //this.Writer.Write(string.Format("<li><b>Method:</b> {1}</li>", "Method"+temp, requestInfo["Method"]));
            this.Writer.WriteLine("<li><b>Header:</b></li>");
            if (!string.IsNullOrEmpty(requestInfo["Headers"]))
            {
                this.Writer.WriteLine(string.Format("<li><textarea readOnly='true'>{0}</textarea></li>", requestInfo["Headers"]));
            }
            isRequestUlOpen = true;
        }

        protected override void PrintRequestData(string data)
        {
            StringBuilder str = new StringBuilder();
            this.Writer.WriteLine("<li><b>Request data:</b></li>");
            if (!string.IsNullOrEmpty(data))
            {
                this.Writer.WriteLine(string.Format("<li><textarea readOnly ='true'>{0}</textarea></li>", data));
            }

            this.Writer.WriteLine(str.ToString());
        }

        protected override void PrintResponse(Dictionary<string, string> responseInfo)
        {
            string temp = "Response" + responseInfo["Id"];
            this.Writer.WriteLine(
                string.Format("<li><input type=\"checkbox\" id=\"{0}\" /><label for=\"{0}\" class=\"response\"><b>Responses:</b></label>", temp));
            this.Writer.WriteLine("<ul>");
            this.Writer.WriteLine(string.Format("<li><b>Http code:</b> {0}</li>", responseInfo["HttpCode"]));
            this.Writer.WriteLine(string.Format("<li><b>Data:</b></li>"));
            if (!string.IsNullOrEmpty(responseInfo["Data"]))
            {
                this.Writer.WriteLine(string.Format("<li><textarea readOnly ='true'>{0}</textarea></li>", responseInfo["Data"]));
            }
            this.Writer.WriteLine("<li><b>Headers:</b></li>");
            if (!string.IsNullOrEmpty(responseInfo["Headers"]))
            {
                this.Writer.WriteLine(string.Format("<li><textarea readOnly ='true'>{0}</textarea></li>", responseInfo["Headers"]));
            }
            this.Writer.WriteLine("</ul></li>");
            if (isRequestUlOpen)
            {
                this.Writer.WriteLine("</ul></li>");
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
        protected void AddStyleSheet(string source)
        {
            this.Writer.WriteLine(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", source));
        }
        /// <summary>
        /// Add Javascript from source
        /// </summary>
        /// <param name="source">javascript source</param>
        protected void AddScript(string source)
        {
            this.Writer.WriteLine(string.Format("<script language=\"JavaScript\" src=\"{0}\"></script>", source));
        }

        /// <summary>
        /// Add preset Toggle buttons: Expanse all, Collapse all
        /// 1. Toggle Debug: Debug, Enter, Exit
        /// 2. Toggle Repro: Repro, Verify, Success
        /// 3. Toggle Warn
        /// 4. Toggle Time
        /// </summary>
        protected virtual void AddToggleButtons()
        {
            //this.Writer.AddAttribute(HtmlTextWriterAttribute.Id, "Buttons");
            this.Writer.WriteLine("<div id=\"Buttons\">");
            AddToggleButton("Expanse all", "Expanse");
            AddToggleButton("Collapse all", "Collapse");
            this.Writer.WriteLine("</div>"); // end Div
        }
        /// <summary>
        /// Add Toggle button for 1 category
        /// </summary>
        /// <param name="name">name of toggle button</param>
        /// <param name="category">log level category</param>
        protected void AddToggleButton(string name, string category)
        {
            string onclick = string.Format("javascript:{0}All()", category);
            string id = string.Format("Toggle {0}", name);
            AddButton(id, id, name, onclick);
        }
        /// <summary>
        /// Add Toggle button for multiple categories
        /// </summary>
        /// <param name="name">name of toggle button</param>
        /// <param name="categories">log level categories</param>
        protected void AddToggleButton(string name, string[] categories)
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
                AddButton(id, id, name, onclick.ToString());
            }
        }

        /// <summary>
        /// Add Go To Button
        /// </summary>
        /// <param name="divId">div Id to go to</param>
        protected void AddGoToButton(string divId)
        {
            string id = string.Format("{0}Button", divId);
            string title = string.Format("Go To {0}", divId);
            string onclick = string.Format("location.href='#{0}'", divId);
            AddButton(id, title, divId, onclick);
        }

        /// <summary>
        /// Add a button
        /// </summary>
        /// <param name="id">input id</param>
        /// <param name="title">input title (hint)</param>
        /// <param name="value">button label</param>
        /// <param name="onclick">what to do onclick</param>
        protected void AddButton(string id, string title, string value, string onclick)
        {
            this.Writer.WriteLine(
                string.Format("<input type=\"button\" id=\"{0}\" title=\"{1}\" value=\"{2}\" onclick=\"{3}\" />",
                id, title, value, onclick));
            //this.Writer.WriteLine();
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
