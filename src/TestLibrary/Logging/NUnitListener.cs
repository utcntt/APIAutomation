using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestLibrary
{
    public class NUnitListener: BaseListener
    {
        public NUnitListener(string header = null, string footer = null)
            : base(header, footer)
        {
            //this.Writer = TestContext.Out;
            
        }

        //public override void Write(string message)
        //{
        //    if(Writer != null)
        //        Writer.Write(message);
        //}

        //public override void WriteLine(string message)
        //{
        //    if (Writer != null)
        //        Writer.WriteLine(message);
        //}

        //public override void WriteLine(string message, string category)
        //{
        //    if ( !category.Equals("testcase")  &&  !category.Equals("fixture")) 
        //    {
        //        base.WriteLine(message, category);
        //    }
        //}
        //protected override void PrintSetUpDescription()
        //{
        //    //base.PrinSetUpDescription();
        //    this.Writer = TestContext.Out;

        //}
        //protected override void PrintTestCaseDescription()
        //{
        //    this.Writer = TestContext.Out;
        //}

        //protected override void PrintFailedTestList(List<Dictionary<string, string>> testList)
        //{
        //    //base.PrintFailedTestList(testList);
        //}

        //protected override void PrintSumary()
        //{
        //    //base.PrintSumary();
        //}

        //protected override void PrintTeardownDescription()
        //{
        //    //base.PrintTeardownDescription();
        //}

        //protected override void PrintTestCaseMessage(string message)
        //{
        //    //base.PrintTestCaseMessage(message);
        //}

        //protected override void PrintTestCaseResult()
        //{
        //    //base.PrintTestCaseResult();
        //}

        //protected override void PrintTestCaseStackTrace(string stackTrace)
        //{
        //    //base.PrintTestCaseStackTrace(stackTrace);
        //}

        //protected override void PrintTestSumary(int total, int failed, int inconclusive, int skipped, int passed)
        //{
        //    //base.PrintTestSumary(total, failed, inconclusive, skipped, passed);
        //}
    }
}
